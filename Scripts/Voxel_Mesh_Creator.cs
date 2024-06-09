using Godot;
using System.Collections.Generic;
using System.Diagnostics;
public partial class Voxel_Mesh_Creator : Node3D
{
	[Export] private Resource itemTextureR;
	[Export] public Node3D meshLocation;
	List<Vector3> facePoss = new List<Vector3>();
	List<Node3D> faceNodes = new List<Node3D>();

	public void GenerateVoxelMesh(ObjectsData objectData)
	{
		if (objectData.object_is_block == false) //build item depending on pixels
		{
			CompressedTexture2D itemTexture = (CompressedTexture2D)objectData.texture_top;
			for (int y = 0; y < itemTexture.GetHeight(); y++)
			{
				for (int x = 0; x < itemTexture.GetWidth(); x++)
				{
					Color pixelColor = itemTexture.GetImage().GetPixel(x, y);
					if (pixelColor.A == 0) continue;
					PixelMesh(pixelColor, new Vector3((float)x * 0.0625f, (float)y * 0.0625f, 0f));
				}
			}
			meshLocation.Position = objectData.object_held_position;
			meshLocation.RotationDegrees = objectData.object_held_rotation;
		}
		else //build block with 6 faces
		{
			BlockMesh(objectData);
			meshLocation.Position = new Vector3(0.195f, -1.29f, -1.425f);
			meshLocation.RotationDegrees = new Vector3(0f, 20f, 0f);
		}
		facePoss.Clear();
		faceNodes.Clear();
	}

	private void PixelMesh(Color _color, Vector3 pixelPos)
	{
		List<Vector3[]> faceVertices = new List<Vector3[]>
		{
			//North
			new Vector3[] { new Vector3(0.0625f, 0.0625f, 0), new Vector3(0, 0.0625f, 0), new Vector3(0, 0, 0), new Vector3(0.0625f, 0, 0) },
			//South
			new Vector3[] { new Vector3(0, 0.0625f, 0.0625f), new Vector3(0.0625f, 0.0625f, 0.0625f), new Vector3(0.0625f, 0, 0.0625f), new Vector3(0, 0, 0.0625f) },
			//West
			new Vector3[] { new Vector3(0, 0.0625f, 0), new Vector3(0, 0.0625f, 0.0625f), new Vector3(0, 0, 0.0625f), new Vector3(0, 0, 0) },
			//East
			new Vector3[] { new Vector3(0.0625f, 0.0625f, 0.0625f), new Vector3(0.0625f, 0.0625f, 0), new Vector3(0.0625f, 0, 0), new Vector3(0.0625f, 0, 0.0625f) },
			//Top
			new Vector3[] { new Vector3(0, 0.0625f, 0), new Vector3(0.0625f, 0.0625f, 0), new Vector3(0.0625f, 0.0625f, 0.0625f), new Vector3(0, 0.0625f, 0.0625f) },
			//Bottom
			new Vector3[] { new Vector3(0, 0, 0.0625f), new Vector3(0.0625f, 0, 0.0625f), new Vector3(0.0625f, 0, 0), new Vector3(0, 0, 0) },
		};

		for (int i = 0; i < 6; i++) //loop through each face
		{
			MeshInstance3D face = new MeshInstance3D();

			ArrayMesh mesh = new ArrayMesh();
			var arrays = new Godot.Collections.Array();
			arrays.Resize((int)ArrayMesh.ArrayType.Max);
			arrays[(int)ArrayMesh.ArrayType.Vertex] = faceVertices[i];
			arrays[(int)ArrayMesh.ArrayType.Index] = new int[] { 0, 1, 2, 0, 2, 3 };

			//Define UV coordinates for the face
			Vector2[] uvs = new Vector2[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
			};
			arrays[(int)ArrayMesh.ArrayType.TexUV] = uvs;


			mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

			//Adding material to face
			StandardMaterial3D material = new StandardMaterial3D();
			if (i == 3 || i == 2)
			{
				Color brighterColor = IncreasePixelColor(_color, 0.0185f);
				material.AlbedoColor = brighterColor;
			}
			else if (i == 4 || i == 5)
			{
				Color brighterColor = IncreasePixelColor(_color, 0.005f);
				material.AlbedoColor = brighterColor;
			}
			else
			{
				material.AlbedoColor = _color;
			}
			material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
			material.DisableReceiveShadows = true;
			material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

			//Set
			face.Mesh = mesh;
			face.MaterialOverride = material;

			//Spawning the cube
			meshLocation.AddChild(face);
			face.Name = "Face_" + i;
			face.Position = new Vector3(face.Position.X + pixelPos.X, face.Position.Y + (-pixelPos.Y), face.Position.Z);

			//checks for duplicate faces and deletes them
			for (int j = 0; j < facePoss.Count; j++)
			{
				if ((face.Position + GetFaceOffset(i)) == facePoss[j])
				{
					face.QueueFree();
					faceNodes[j].QueueFree();
					facePoss.RemoveAt(j);
					faceNodes.RemoveAt(j);
				}
			}

			facePoss.Add(face.Position + GetFaceOffset(i));
			faceNodes.Add(face);
		}
	}

	private Vector3 GetFaceOffset(int faceIndex)
	{
		switch (faceIndex)
		{
			case 0: return new Vector3(0, 0, -0.03125f); // North face offset
			case 1: return new Vector3(0, 0, 0.03125f); // South face offset
			case 2: return new Vector3(-0.03125f, 0, 0); // West face offset
			case 3: return new Vector3(0.03125f, 0, 0); // East face offset
			case 4: return new Vector3(0, 0.03125f, 0); // Top face offset
			case 5: return new Vector3(0, -0.03125f, 0); // Bottom face offset
			default: return Vector3.Zero;
		}
	}

	private Color IncreasePixelColor(Color color, float factor = 0.1f)
	{
		factor = Mathf.Clamp(factor, 0.0f, 1.0f);

		float r = Mathf.Clamp(color.R + factor, 0.0f, 1.0f);
		float g = Mathf.Clamp(color.G + factor, 0.0f, 1.0f);
		float b = Mathf.Clamp(color.B + factor, 0.0f, 1.0f);

		return new Color(r, g, b, color.A);
	}

	public void RemoveMesh()
	{
		foreach (Node child in meshLocation.GetChildren())
		{
			child.QueueFree();
		}
	}

	public void BlockMesh(ObjectsData _blockData)
	{
		ObjectsData currentBlockData = _blockData;

		// Define vertices for each face
		List<Vector3[]> faceVertices = new List<Vector3[]>
		{
			// North face
			new Vector3[] { new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0) },
			// South face
			new Vector3[] { new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 0, 1), new Vector3(0, 0, 1) },
			// West face
			new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0) },
			// East face
			new Vector3[] { new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1) },
			// Top face
			new Vector3[] { new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1) },
			// Bottom face
			new Vector3[] { new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 0) },
		};

		// Define textures for each face
		List<Texture2D> textures = new List<Texture2D>
		{
			currentBlockData.texture_front,
			currentBlockData.texture_back,
			currentBlockData.texture_left,
			currentBlockData.texture_right,
			currentBlockData.texture_top,
			currentBlockData .texture_bottom,
		};

		// Loop through each face, create a child MeshInstance, and assign the corresponding material
		for (int i = 0; i < 6; i++)
		{
			// Create a new MeshInstance for the face
			MeshInstance3D face = new MeshInstance3D();

			// Create the mesh for the face
			ArrayMesh mesh = new ArrayMesh();
			var arrays = new Godot.Collections.Array();
			arrays.Resize((int)ArrayMesh.ArrayType.Max);
			arrays[(int)ArrayMesh.ArrayType.Vertex] = faceVertices[i];
			arrays[(int)ArrayMesh.ArrayType.Index] = new int[] { 0, 1, 2, 0, 2, 3 };

			// Define UV coordinates for the face
			Vector2[] uvs = new Vector2[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
			};
			arrays[(int)ArrayMesh.ArrayType.TexUV] = uvs;

			// Add surface to mesh
			mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

			// Create the material for the face
			StandardMaterial3D material = new StandardMaterial3D();
			material.AlbedoTexture = textures[i];
			material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
			material.DisableReceiveShadows = true;
			material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

			// Set the mesh and material
			face.Mesh = mesh;
			face.MaterialOverride = material;

			// Add the face as a child of the Cube
			meshLocation.AddChild(face);
		}
	}

}
