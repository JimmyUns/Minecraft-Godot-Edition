using Godot;
using System;

public partial class Held_Object_Maker : Node3D
{
	[Export] public MeshInstance3D meshInstance;
	[Export] public Node3D meshOffset;
	
	[Export] public MeshInstance3D heldobjectMeshInstance;
	
	[Export] public AnimationPlayer switchheldAnim;
	public ArrayMesh handMesh;


	public static Vector3I dimensions = new Vector3I(16, 64, 16);

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


	public void CreateBlockMesh(Block block)
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateFaceMesh(_top, block.Texture_Top);
		CreateFaceMesh(_bottom, block.Texture_Bottom ?? block.Texture_Top);
		CreateFaceMesh(_left, block.Texture_Left ?? block.Texture_Top);
		CreateFaceMesh(_right, block.Texture_Right ?? block.Texture_Top);
		CreateFaceMesh(_front, block.Texture_Forward ?? block.Texture_Top);
		CreateFaceMesh(_back, block.Texture_Back ?? block.Texture_Top);

		_surfaceTool.SetMaterial(Block_Manager.Instance.chunkMaterial); //Use the chunk Material

		var mesh = _surfaceTool.Commit();
		meshInstance.Mesh = mesh;
		heldobjectMeshInstance.Mesh = meshInstance.Mesh;
	}

	private void CreateFaceMesh(int[] face, Texture2D texture)
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

		var a = _vertices[face[0]] + Vector3.Zero;
		var b = _vertices[face[1]] + Vector3.Zero;
		var c = _vertices[face[2]] + Vector3.Zero;
		var d = _vertices[face[3]] + Vector3.Zero;

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

	public void SetHandMesh()
	{
		meshInstance.Mesh = handMesh;
		heldobjectMeshInstance.Mesh = null;
	}


	
}
