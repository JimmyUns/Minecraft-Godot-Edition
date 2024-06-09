using Godot;
using System;
using System.Collections.Generic;

public partial class Block_Mesh : MeshInstance3D
{
	public void UpdateMesh(ObjectsData _blockData)
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
			AddChild(face);
		}
	}
}
