using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player_Mesh_Maker : Node
{
	[Export] public Node3D playerManager;
	private Texture2D skin;
	[Export] public MeshInstance3D head, body, armL, armR, legL, legR;
	[Export] public MeshInstance3D headOL, bodyOL, armLOL, armROL, legLOL, legROL;
	[Export] public Node3D armROverride;
	private SurfaceTool _surfaceTool = new SurfaceTool(); //Tool that creates the 3dShape
	Texture2D[] skinTextures = new Texture2D[12];

	private StandardMaterial3D headMaterial, bodyMaterial, armLMaterial, armRMaterial, legLMaterial, legRMaterial;
	private StandardMaterial3D headMaterialOL, bodyMaterialOL, armLMaterialOL, armRMaterialOL, legLMaterialOL, legRMaterialOL;


	public override void _Ready()
	{
		skin = Game_manager.instance.skin;
		LoadTextures();
		CreateHead();
		CreateBody();
		CreateArmLeft();
		CreateArmRight();
		CreateLegLeft();
		CreateLegRight();
		
		(playerManager as Player_Manager).paperdollController.CreatePaperdoll();
	}

	private void LoadTextures()
	{
		for (int i = 0; i < skinTextures.Length; i++)
		{
			skinTextures[i] = ImageTexture.CreateFromImage(skin.GetImage().GetRegion(texturesRect[i]));
		}
		StandardMaterial3D awd0 = new StandardMaterial3D()
		{
			AlbedoTexture = skin,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			Transparency = BaseMaterial3D.TransparencyEnum.Disabled
		};

		StandardMaterial3D awd1 = new StandardMaterial3D()
		{
			AlbedoTexture = skin,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			CullMode = BaseMaterial3D.CullModeEnum.Back
		};

		headMaterial = awd0;
		bodyMaterial = awd0;
		armLMaterial = awd0;
		armRMaterial = awd0;
		legLMaterial = awd0;
		legRMaterial = awd0;
		headMaterialOL = awd1;
		bodyMaterialOL = awd1;
		armLMaterialOL = awd1;
		armRMaterialOL = awd1;
		legLMaterialOL = awd1;
		legRMaterialOL = awd1;
	}

	private void CreateMesh(int[] face, Rect2I region, int bodyArea, int horizontalIndex)
	{
		var UV_A = region.Position / skin.GetSize(); //top left
		var UV_B = (region.Position + new Vector2(region.Size.X, 0)) / skin.GetSize(); //bottom left
		var UV_C = (region.Position + region.Size) / skin.GetSize(); //bottom right
		var UV_D = (region.Position + new Vector2(0, region.Size.Y)) / skin.GetSize(); //top right

		Vector3 a = new();
		Vector3 b = new();
		Vector3 c = new();
		Vector3 d = new();

		//0:head 1:body 2:hands/legs
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

			case 2:
				Vector3 awd2 = new Vector3(0.249f, 0.31f, 0.1245f);
				a = _armvertices[face[0]] - awd2;
				b = _armvertices[face[1]] - awd2;
				c = _armvertices[face[2]] - awd2;
				d = _armvertices[face[3]] - awd2;
				break;
		}

		_ = new Vector2[3];
		_ = new Vector2[3];
		Vector2[] UV_Triangle1;
		Vector2[] UV_Triangle2;

		switch (horizontalIndex)
		{
			case 1:
				UV_Triangle1 = new Vector2[] { UV_C, UV_D, UV_A };
				UV_Triangle2 = new Vector2[] { UV_C, UV_A, UV_B };
				break;
			case 2:
				UV_Triangle1 = new Vector2[] { UV_C, UV_B, UV_A };
				UV_Triangle2 = new Vector2[] { UV_C, UV_A, UV_D };
				break;
			default:
				UV_Triangle1 = new Vector2[] { UV_B, UV_C, UV_D };
				UV_Triangle2 = new Vector2[] { UV_B, UV_D, UV_A };
				break;
		}

		var triangle_1 = new Vector3[] { a, b, c };
		var triangle_2 = new Vector3[] { a, c, d };

		//Create Normals
		var normal = ((Vector3)(c - a)).Cross((Vector3)(b - a)).Normalized();
		var normals = new Vector3[] { normal, normal, normal };

		_surfaceTool.AddTriangleFan(triangle_1, UV_Triangle1, normals: normals);
		_surfaceTool.AddTriangleFan(triangle_2, UV_Triangle2, normals: normals);
	}

	private static readonly Rect2I[] texturesRect = new Rect2I[] //Starting top left corner and moving right, 0:head 6:body 12:armL
	{
		//Head
		new Rect2I(8, 0, 8, 8), //Top
		new Rect2I(16, 0, 8, 8), //Bot
		new Rect2I(16, 8, 8, 8), //Our Right
		new Rect2I(8, 8, 8, 8), //Forward
		new Rect2I(0, 8, 8, 8), //Our Left
		new Rect2I(24, 8, 8, 8), //Backward

		//Body
		new Rect2I(20, 16, 8, 4), //Top
		new Rect2I(28, 16, 8, 4), //Bot
		new Rect2I(28, 20, 4, 12), //Our Left
		new Rect2I(20, 20, 8, 12), //Forward
		new Rect2I(16, 20, 4, 12), //Our Right
		new Rect2I(32, 20, 8, 12), //Backward
		
		//Right Arm
		new Rect2I(44, 16, 4, 4), //Top
		new Rect2I(48, 16, 4, 4), //Bot
		new Rect2I(48, 20, 4, 12), //Our Left
		new Rect2I(44, 20, 4, 12), //Our Right
		new Rect2I(40, 20, 4, 12), //Forward
		new Rect2I(52, 20, 4, 12), //Backward
		
		//Left Arm
		new Rect2I(36, 48, 4, 4), //Top
		new Rect2I(40, 48, 4, 4), //Bot
		new Rect2I(40, 52, 4, 12), //Our Left
		new Rect2I(36, 52, 4, 12), //Our Right
		new Rect2I(32, 52, 4, 12), //Forward
		new Rect2I(44, 52, 4, 12), //Backward
		
		//Right Leg
		new Rect2I(4, 16, 4, 4), //Top
		new Rect2I(8, 16, 4, 4), //Bot
		new Rect2I(8, 20, 4, 12), //Our Right
		new Rect2I(4, 20, 4, 12), //Forward
		new Rect2I(0, 20, 4, 12), //Our Left
		new Rect2I(12, 20, 4, 12), //Backward
		
		//Left Leg
		new Rect2I(20, 48, 4, 4), //Top
		new Rect2I(24, 48, 4, 4), //Bot
		new Rect2I(24, 52, 4, 12), //Our Right
		new Rect2I(20, 52, 4, 12), //Forward
		new Rect2I(16, 52, 4, 12), //Our Left
		new Rect2I(28, 52, 4, 12), //Backward
	};

	private static readonly Rect2I[] texturesOLRect = new Rect2I[] //Starting top left corner and moving right, 0:head 6:body 12:armL
	{
		//Head
		new Rect2I(40, 0, 8, 8), //Top
		new Rect2I(48, 0, 8, 8), //Bot
		new Rect2I(32, 8, 8, 8), //Our Left
		new Rect2I(40, 8, 8, 8), //Forward
		new Rect2I(48, 8, 8, 8), //Our Right
		new Rect2I(56, 8, 8, 8), //Backward

		//Body
		new Rect2I(20, 32, 8, 4), //Top
		new Rect2I(28, 32, 8, 4), //Bot
		new Rect2I(28, 36, 4, 12), //Our Left
		new Rect2I(20, 36, 8, 12), //Forward
		new Rect2I(16, 36, 4, 12), //Our Right
		new Rect2I(32, 36, 8, 12), //Backward
		
		//Left Arm
		new Rect2I(52, 48, 4, 4), //Top
		new Rect2I(56, 48, 4, 4), //Bot
		new Rect2I(56, 52, 4, 12), //Our Left
		new Rect2I(52, 52, 4, 12), //Our Right
		new Rect2I(48, 52, 4, 12), //Forward
		new Rect2I(60, 52, 4, 12), //Backward
		
		//Right Arm
		new Rect2I(44, 32, 4, 4), //Top
		new Rect2I(48, 32, 4, 4), //Bot
		new Rect2I(48, 36, 4, 12), //Our Left
		new Rect2I(44, 36, 4, 12), //Our Right
		new Rect2I(40, 36, 4, 12), //Forward
		new Rect2I(52, 36, 4, 12), //Backward
		
		//Right Leg
		new Rect2I(4, 48, 4, 4), //Top
		new Rect2I(8, 48, 4, 4), //Bot
		new Rect2I(8, 52, 4, 12), //Our Right
		new Rect2I(4, 52, 4, 12), //Forward
		new Rect2I(0, 52, 4, 12), //Our Left
		new Rect2I(12, 52, 4, 12), //Backward
		
		//Left Leg
		new Rect2I(4, 32, 4, 4), //Top
		new Rect2I(8, 32, 4, 4), //Bot
		new Rect2I(8, 36, 4, 12), //Our Right
		new Rect2I(4, 36, 4, 12), //Forward
		new Rect2I(0, 36, 4, 12), //Our Left
		new Rect2I(12, 36, 4, 12), //Backward
		
	};

	#region Create Body Parts
	private void CreateHead()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[0], 0, 1);
		CreateMesh(_bottom, texturesRect[1], 0, 2);
		CreateMesh(_left, texturesRect[2], 0, 0);
		CreateMesh(_right, texturesRect[4], 0, 0);
		CreateMesh(_front, texturesRect[3], 0, 0);
		CreateMesh(_back, texturesRect[5], 0, 0);

		_surfaceTool.SetMaterial(headMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		head.Mesh = mesh;

		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[0], 0, 1);
		CreateMesh(_bottom, texturesOLRect[1], 0, 2);
		CreateMesh(_left, texturesOLRect[2], 0, 0);
		CreateMesh(_right, texturesOLRect[4], 0, 0);
		CreateMesh(_front, texturesOLRect[3], 0, 0);
		CreateMesh(_back, texturesOLRect[5], 0, 0);

		_surfaceTool.SetMaterial(headMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		headOL.Mesh = meshOL;
	}

	private void CreateBody()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[6], 1, 1);
		CreateMesh(_bottom, texturesRect[7], 1, 2);
		CreateMesh(_left, texturesRect[8], 1, 0);
		CreateMesh(_right, texturesRect[10], 1, 0);
		CreateMesh(_front, texturesRect[9], 1, 0);
		CreateMesh(_back, texturesRect[11], 1, 0);


		_surfaceTool.SetMaterial(bodyMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		body.Mesh = mesh;

		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[6], 1, 1);
		CreateMesh(_bottom, texturesOLRect[7], 1, 2);
		CreateMesh(_left, texturesOLRect[8], 1, 0);
		CreateMesh(_right, texturesOLRect[10], 1, 0);
		CreateMesh(_front, texturesOLRect[9], 1, 0);
		CreateMesh(_back, texturesOLRect[11], 1, 0);

		_surfaceTool.SetMaterial(bodyMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		bodyOL.Mesh = meshOL;
	}

	private void CreateArmLeft()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[12], 2, 1);
		CreateMesh(_bottom, texturesRect[13], 2, 2);
		CreateMesh(_left, texturesRect[14], 2, 0);
		CreateMesh(_right, texturesRect[16], 2, 0);
		CreateMesh(_front, texturesRect[15], 2, 0);
		CreateMesh(_back, texturesRect[17], 2, 0);


		_surfaceTool.SetMaterial(armLMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		armL.Mesh = mesh;

		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[12], 2, 1);
		CreateMesh(_bottom, texturesOLRect[13], 2, 2);
		CreateMesh(_left, texturesOLRect[14], 2, 0);
		CreateMesh(_right, texturesOLRect[16], 2, 0);
		CreateMesh(_front, texturesOLRect[15], 2, 0);
		CreateMesh(_back, texturesOLRect[17], 2, 0);


		_surfaceTool.SetMaterial(armLMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		armLOL.Mesh = meshOL;
	}

	private void CreateArmRight()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[18], 2, 1);
		CreateMesh(_bottom, texturesRect[19], 2, 2);
		CreateMesh(_left, texturesRect[20], 2, 0);
		CreateMesh(_right, texturesRect[22], 2, 0);
		CreateMesh(_front, texturesRect[21], 2, 0);
		CreateMesh(_back, texturesRect[23], 2, 0);


		_surfaceTool.SetMaterial(armRMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		armR.Mesh = mesh;

		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[18], 2, 1);
		CreateMesh(_bottom, texturesOLRect[19], 2, 2);
		CreateMesh(_left, texturesOLRect[20], 2, 0);
		CreateMesh(_right, texturesOLRect[22], 2, 0);
		CreateMesh(_front, texturesOLRect[21], 2, 0);
		CreateMesh(_back, texturesOLRect[23], 2, 0);

		_surfaceTool.SetMaterial(armRMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		armROL.Mesh = meshOL;
		(playerManager as Player_Manager).heldObjectMaker.handMesh = mesh;
	}

	private void CreateLegLeft()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[30], 2, 1);
		CreateMesh(_bottom, texturesRect[31], 2, 2);
		CreateMesh(_left, texturesRect[32], 2, 0);
		CreateMesh(_right, texturesRect[34], 2, 0);
		CreateMesh(_front, texturesRect[33], 2, 0);
		CreateMesh(_back, texturesRect[35], 2, 0);

		_surfaceTool.SetMaterial(legLMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		legL.Mesh = mesh;
		
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[30], 2, 1);
		CreateMesh(_bottom, texturesOLRect[31], 2, 2);
		CreateMesh(_left, texturesOLRect[32], 2, 0);
		CreateMesh(_right, texturesOLRect[34], 2, 0);
		CreateMesh(_front, texturesOLRect[33], 2, 0);
		CreateMesh(_back, texturesOLRect[35], 2, 0);

		_surfaceTool.SetMaterial(legLMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		legROL.Mesh = meshOL;
	}

	private void CreateLegRight()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesRect[24], 2, 1);
		CreateMesh(_bottom, texturesRect[25], 2, 2);
		CreateMesh(_left, texturesRect[26], 2, 0);
		CreateMesh(_right, texturesRect[28], 2, 0);
		CreateMesh(_front, texturesRect[27], 2, 0);
		CreateMesh(_back, texturesRect[29], 2, 0);

		_surfaceTool.SetMaterial(legRMaterial); // Use the chunk Material

		var mesh = _surfaceTool.Commit();
		legR.Mesh = mesh;
		
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		CreateMesh(_top, texturesOLRect[24], 2, 1);
		CreateMesh(_bottom, texturesOLRect[25], 2, 2);
		CreateMesh(_left, texturesOLRect[26], 2, 0);
		CreateMesh(_right, texturesOLRect[28], 2, 0);
		CreateMesh(_front, texturesOLRect[27], 2, 0);
		CreateMesh(_back, texturesOLRect[29], 2, 0);

		_surfaceTool.SetMaterial(legRMaterialOL); // Use the chunk Material

		var meshOL = _surfaceTool.Commit();
		legLOL.Mesh = meshOL;
	}
	#endregion

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

	private static readonly Vector3[] _armvertices = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(0.25f, 0f, 0f),
		new Vector3(0, 0.75f, 0),
		new Vector3(0.25f, 0.75f, 0),
		new Vector3(0, 0, 0.25f),
		new Vector3(0.25f, 0, 0.25f),
		new Vector3(0, 0.75f, 0.25f),
		new Vector3(0.25f, 0.75f, 0.25f)
	};

	private static readonly int[] _top = new int[] { 2, 3, 7, 6 };
	private static readonly int[] _bottom = new int[] { 0, 4, 5, 1 };
	private static readonly int[] _left = new int[] { 6, 4, 0, 2 };
	private static readonly int[] _right = new int[] { 3, 1, 5, 7 };
	private static readonly int[] _back = new int[] { 7, 5, 4, 6 };
	private static readonly int[] _front = new int[] { 2, 0, 1, 3 };
	#endregion
}
