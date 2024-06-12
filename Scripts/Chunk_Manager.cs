using Godot;
using System;
using System.Diagnostics;

public partial class Chunk_Manager : StaticBody3D
{
	[Export] public CollisionShape3D colShape;
	[Export] public MeshInstance3D meshInstance;

	public static Vector3I dimensions = new Vector3I(16, 128, 16);

	private static readonly Vector3I[] _vertices = new Vector3I[]
	{
		new Vector3I(0, 0, 0),
		new Vector3I(1, 0, 0),
		new Vector3I(0, 1, 0),
		new Vector3I(1, 1, 0),
		new Vector3I(0, 0, 1),
		new Vector3I(1, 0, 1),
		new Vector3I(0, 1, 1),
		new Vector3I(1, 1, 1)
	};

	private static readonly int[] _top = new int[] { 2, 3, 7, 6 };
	private static readonly int[] _bottom = new int[] { 0, 4, 5, 1 };
	private static readonly int[] _left = new int[] { 6, 4, 0, 2 };
	private static readonly int[] _right = new int[] { 3, 1, 5, 7 };
	private static readonly int[] _back = new int[] { 7, 5, 4, 6 };
	private static readonly int[] _front = new int[] { 2, 0, 1, 3 };

	private SurfaceTool _surfaceTool = new SurfaceTool();//Tool that creats the 3dShape

	private Block[,,] _blocks = new Block[dimensions.X, dimensions.Y, dimensions.Z];

	//chunk location in the chunk list
	public Vector2I chunkPosition { get; private set; }

	[Export] public FastNoiseLite Noise;

	public void SetChunkPosition(Vector2I position)
	{
		Chunk_World_Manager.instance.UpdateChunkPosition(this, position, chunkPosition);
		chunkPosition = position;
		//Because in chunk_world_manager its also being calleddefered so that we set the position of the chunk after creating it
		CallDeferred(Node3D.MethodName.SetGlobalPosition, new Godot.Vector3(chunkPosition.X * dimensions.X, 0, chunkPosition.Y * dimensions.Z));

		Generate();
		Update();
	}

	//Generate Mesh Chunk
	public void Generate()
	{
		for (int x = 0; x < dimensions.X; x++)
		{
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int z = 0; z < dimensions.Z; z++)
				{
					Block block;

					var globalBlockPosition = chunkPosition * new Vector2I(dimensions.X, dimensions.Z) + new Vector2I(x, z);
					var groundHeight = (int)(dimensions.Y * ((Noise.GetNoise2D(globalBlockPosition.X, globalBlockPosition.Y) + 1f) / 2f)); //World Height 0 to dimensions.y which is 64

					if (y == 0)
					{
						block = Block_Manager.Instance.Bedrock;
					}
					else if (y < groundHeight - 3)
					{
						block = Block_Manager.Instance.Stone;
					}
					else if (y < groundHeight)
					{
						block = Block_Manager.Instance.Dirt;
					}
					else if (y == groundHeight)
					{
						block = Block_Manager.Instance.Grass;
					}
					else
					{
						block = Block_Manager.Instance.Air;
					}
					_blocks[x, y, z] = block;
				}
			}
		}
	}

	//Generate Collision and mesh for chunk
	public void Update()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < dimensions.X; x++)
		{
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int z = 0; z < dimensions.Z; z++)
				{
					CreateBlockMesh(new Vector3I(x, y, z));
				}
			}
		}

		_surfaceTool.SetMaterial(Block_Manager.Instance.chunkMaterial); //Use the chunk Material

		var mesh = _surfaceTool.Commit();
		meshInstance.Mesh = mesh;
		colShape.Shape = mesh.CreateTrimeshShape();
	}

	private void CreateBlockMesh(Vector3I bPos)
	{
		var block = _blocks[bPos.X, bPos.Y, bPos.Z];

		if (block == Block_Manager.Instance.Air) return; //remove if need to add lighting

		if (CheckTransparent(bPos + Vector3I.Up))
		{
			CreateFaceMesh(_top, bPos, block.Texture_Top);
		}
		if (CheckTransparent(bPos + Vector3I.Down))
		{
			CreateFaceMesh(_bottom, bPos, block.Texture_Bottom ?? block.Texture_Top);
		}
		if (CheckTransparent(bPos + Vector3I.Left))
		{
			CreateFaceMesh(_left, bPos, block.Texture_Left ?? block.Texture_Top);
		}
		if (CheckTransparent(bPos + Vector3I.Right))
		{
			CreateFaceMesh(_right, bPos, block.Texture_Right ?? block.Texture_Top);
		}
		if (CheckTransparent(bPos + Vector3I.Forward))
		{
			CreateFaceMesh(_front, bPos, block.Texture_Forward ?? block.Texture_Top);
		}
		if (CheckTransparent(bPos + Vector3I.Back))
		{
			CreateFaceMesh(_back, bPos, block.Texture_Back ?? block.Texture_Top);
		}
	}

	private void CreateFaceMesh(int[] face, Vector3I bPos, Texture2D texture)
	{
		//Setting UV's
		var texturePosition = Block_Manager.Instance.GetTextureAtlastPosition(texture);
		var textureAtlasSize = Block_Manager.Instance.textureAtlasSize;

		var UV_Offset = texturePosition / textureAtlasSize;
		float UV_Width = 1f / textureAtlasSize.X;
		float UV_Height = 1f / textureAtlasSize.Y;

		var UV_A = UV_Offset + new Vector2(0, 0);
		var UV_B = UV_Offset + new Vector2(0, UV_Height);
		var UV_C = UV_Offset + new Vector2(UV_Width, UV_Height);
		var UV_D = UV_Offset + new Vector2(UV_Width, 0);

		//Setting Mesh
		var a = _vertices[face[0]] + bPos;
		var b = _vertices[face[1]] + bPos;
		var c = _vertices[face[2]] + bPos;
		var d = _vertices[face[3]] + bPos;

		var UV_Triangle1 = new Vector2[] { UV_A, UV_B, UV_C };
		var UV_Triangle2 = new Vector2[] { UV_A, UV_C, UV_D };

		var triangle_1 = new Vector3[] { a, b, c };
		var triangle_2 = new Vector3[] { a, c, d };

		//Create Normals
		var normal = ((Vector3)(c - a)).Cross((Vector3)(b - a)).Normalized();
		var normals = new Vector3[] { normal, normal, normal };

		_surfaceTool.AddTriangleFan(triangle_1, UV_Triangle1, normals: normals);
		_surfaceTool.AddTriangleFan(triangle_2, UV_Triangle2, normals: normals);
	}

	private bool CheckTransparent(Vector3I bPos)
	{
		if (bPos.X < 0 || bPos.X >= dimensions.X) return true;
		if (bPos.Y < 0 || bPos.Y >= dimensions.Y) return true;
		if (bPos.Z < 0 || bPos.Z >= dimensions.Z) return true;

		//Adds transparent blocks
		return _blocks[bPos.X, bPos.Y, bPos.Z] == Block_Manager.Instance.Air;
	}

	public void SetBlock(Vector3I bPos, Block block)
	{
		_blocks[bPos.X, bPos.Y, bPos.Z] = block;
		Update();
	}
}
