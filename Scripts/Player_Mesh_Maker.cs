using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player_Mesh_Maker : Node
{
	[Export] public TextureRect textureVisualse;
	[Export] public CompressedTexture2D skin;
	[Export] private MeshInstance3D head, body, armL, armR, legL, legR;
	private SurfaceTool _surfaceTool = new SurfaceTool();//Tool that creats the 3dShape

	private int _gridWidth = 4;
	private int _gridHeight;
	public Vector2 textureAtlasSize { get; private set; }
	private readonly Dictionary<Texture2D, Vector2I> _atlasLookup = new Dictionary<Texture2D, Vector2I>();
	public StandardMaterial3D headMaterial { get; private set; }
	public StandardMaterial3D bodyMaterial { get; private set; }
	
	Texture2D[] skinTextures = new Texture2D[12];


	public override void _Ready()
	{
		CreateHeadAtlas();//Seperate the images and put them in an atlas
		CreateBodyAtlas();
		CreateHead();
		CreateBody();
	}

	private void CreateHeadAtlas()
	{
		for (int i = 0; i < skinTextures.Length; i++)
		{
			skinTextures[i] = ImageTexture.CreateFromImage(skin.GetImage().GetRegion(headSeperateTextures[i]));
		}

		Vector2I skinTextureSize = new Vector2I(8, 8);

		for (int i = 0; i < skinTextures.Length; i++)
		{
			Texture2D texture = skinTextures[i];
			_atlasLookup.Add(texture, new Vector2I(i % _gridWidth, Mathf.FloorToInt(i / _gridWidth)));
		}

		_gridHeight = Mathf.CeilToInt(skinTextures.Length / (float)_gridWidth); //if we have an image and grid is 4, we create a second row with only 1 cell

		Image image = Image.Create(_gridWidth * skinTextureSize.X, _gridHeight * skinTextureSize.Y, false, Image.Format.Rgb8); //Create the actual image

		for (int x = 0; x < _gridWidth; x++)
		{
			for (int y = 0; y < _gridHeight; y++)
			{
				int imgIndex = x + y * _gridWidth;

				if (imgIndex >= skinTextures.Length) continue;

				Image currentImage = skinTextures[imgIndex].GetImage();
				currentImage.Convert(Image.Format.Rgb8);

				image.BlitRect(currentImage, new Rect2I(Vector2I.Zero, skinTextureSize), new Vector2I(x, y) * skinTextureSize);
			}
		}

		ImageTexture textureAtlas = ImageTexture.CreateFromImage(image);

		headMaterial = new StandardMaterial3D()
		{
			AlbedoTexture = textureAtlas,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
		};

		textureAtlasSize = new Vector2(_gridWidth, _gridHeight);
	}
	
	private void CreateBodyAtlas()
	{
		for (int i = 0; i < skinTextures.Length; i++)
		{
			skinTextures[i] = ImageTexture.CreateFromImage(skin.GetImage().GetRegion(headSeperateTextures[i]));
		}

		Vector2I skinTextureSize = new Vector2I(8, 8);

		for (int i = 0; i < skinTextures.Length; i++)
		{
			Texture2D texture = skinTextures[i];
			_atlasLookup.Add(texture, new Vector2I(i % _gridWidth, Mathf.FloorToInt(i / _gridWidth)));
		}

		_gridHeight = Mathf.CeilToInt(skinTextures.Length / (float)_gridWidth); //if we have an image and grid is 4, we create a second row with only 1 cell

		Image image = Image.Create(_gridWidth * skinTextureSize.X, _gridHeight * skinTextureSize.Y, false, Image.Format.Rgb8); //Create the actual image

		for (int x = 0; x < _gridWidth; x++)
		{
			for (int y = 0; y < _gridHeight; y++)
			{
				int imgIndex = x + y * _gridWidth;

				if (imgIndex >= skinTextures.Length) continue;

				Image currentImage = skinTextures[imgIndex].GetImage();
				currentImage.Convert(Image.Format.Rgb8);

				image.BlitRect(currentImage, new Rect2I(Vector2I.Zero, skinTextureSize), new Vector2I(x, y) * skinTextureSize);
			}
		}

		ImageTexture textureAtlas = ImageTexture.CreateFromImage(image);

		bodyMaterial = new StandardMaterial3D()
		{
			AlbedoTexture = textureAtlas,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
		};

		textureAtlasSize = new Vector2(_gridWidth, _gridHeight);
	}

	private void CreateHead()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateFaceMesh(_top, skinTextures[0], 0);
		CreateFaceMesh(_bottom, skinTextures[1], 0);
		CreateFaceMesh(_left, skinTextures[2], 0);
		CreateFaceMesh(_right, skinTextures[4], 0);
		CreateFaceMesh(_front, skinTextures[3], 0);
		CreateFaceMesh(_back, skinTextures[5], 0);

		_surfaceTool.SetMaterial(headMaterial); //Use the chunk Material

		var mesh = _surfaceTool.Commit();
		head.Mesh = mesh;
	}

	private void CreateBody()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateFaceMesh(_top, skinTextures[6], 1);
		CreateFaceMesh(_bottom, skinTextures[7], 1);
		CreateFaceMesh(_left, skinTextures[8], 1);
		CreateFaceMesh(_right, skinTextures[10], 1);
		CreateFaceMesh(_front, skinTextures[9], 1);
		CreateFaceMesh(_back, skinTextures[11], 1);
		
		_surfaceTool.SetMaterial(bodyMaterial); //Use the chunk Material

		var mesh = _surfaceTool.Commit();
		body.Mesh = mesh;
	}

	private void CreateFaceMesh(int[] face, Texture2D texture, int bodyArea)
	{
		//Setting UV's
		var texturePosition = GetTextureAtlastPosition(texture);

		var UV_Offset = texturePosition / textureAtlasSize;
		float UV_Width = 1f / textureAtlasSize.X;
		float UV_Height = 1f / textureAtlasSize.Y;

		var UV_A = UV_Offset + new Vector2(0, 0);
		var UV_B = UV_Offset + new Vector2(0, UV_Height);
		var UV_C = UV_Offset + new Vector2(UV_Width, UV_Height);
		var UV_D = UV_Offset + new Vector2(UV_Width, 0);


		Vector3 a = new();
		Vector3 b = new();
		Vector3 c = new();
		Vector3 d = new();

		//0:head 1:body 2:armL 3:armR 4:legL 5:legR	
		switch (bodyArea)
		{
			case 0:
				Vector3 awd0 = new Vector3(0.249f, 0.226f, 0.249f);
				a = _headvertices[face[0]] - awd0;
				b = _headvertices[face[1]] - awd0;
				c = _headvertices[face[2]] - awd0;
				d = _headvertices[face[3]] - awd0;
				break;

			case 1:
				Vector3 awd1 = new Vector3(0.249f, 0.31f, 0.1245f);
				a = _bodyvertices[face[0]] - awd1;
				b = _bodyvertices[face[1]] - awd1;
				c = _bodyvertices[face[2]] - awd1;
				d = _bodyvertices[face[3]] - awd1;
				break;
		}

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
	public Vector2I GetTextureAtlastPosition(Texture2D texture)
	{
		if (texture == null) return Vector2I.Zero;

		return _atlasLookup[texture];
	}
	private static readonly Rect2I[] headSeperateTextures = new Rect2I[] //Starting top left corner and moving right
	{
		new Rect2I(8, 0, 8, 8), //Top
		new Rect2I(16, 0, 8, 8), //Bot
		new Rect2I(0, 8, 8, 8), //Our Left
		new Rect2I(8, 8, 8, 8), //Forward
		new Rect2I(16, 8, 8, 8), //Our Right
		new Rect2I(24, 8, 8, 8), //Backward


		new Rect2I(20, 16, 4, 8), //Top
		new Rect2I(28, 16, 4, 8), //Bot
		new Rect2I(16, 20, 4, 8), //Our Left
		new Rect2I(20, 20, 8, 12), //Forward
		new Rect2I(28, 20, 4, 8), //Our Right
		new Rect2I(32, 20, 8, 12), //Backward
	};

	#region Vertices
	private static readonly Vector3[] _headvertices = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(0.5f, 0f, 0f),
		new Vector3(0, 0.5f, 0),
		new Vector3(0.5f, 0.5f, 0),
		new Vector3(0, 0, 0.5f),
		new Vector3(0.5f, 0, 0.5f),
		new Vector3(0, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f)
	};

	private static readonly Vector3[] _bodyvertices = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(0.5f, 0f, 0f),
		new Vector3(0, 0.75f, 0),
		new Vector3(0.5f, 0.75f, 0),
		new Vector3(0, 0, 0.25f),
		new Vector3(0.5f, 0, 0.25f),
		new Vector3(0, 0.75f, 0.25f),
		new Vector3(0.5f, 0.75f, 0.25f)
	};

	private static readonly int[] _top = new int[] { 2, 3, 7, 6 };
	private static readonly int[] _bottom = new int[] { 0, 4, 5, 1 };
	private static readonly int[] _left = new int[] { 6, 4, 0, 2 };
	private static readonly int[] _right = new int[] { 3, 1, 5, 7 };
	private static readonly int[] _back = new int[] { 7, 5, 4, 6 };
	private static readonly int[] _front = new int[] { 2, 0, 1, 3 };
	#endregion

}
