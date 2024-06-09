using Godot;
using System;

public partial class Block_Manager2 : StaticBody3D
{
	[Export] public ObjectsData currentObjectData;
	[Export] public Block_Mesh blockMesh;
	
	public override void _Ready()
	{
		blockMesh.UpdateMesh(currentObjectData);
	}
	
	public void UpdateBlockData()
	{
		blockMesh.UpdateMesh(currentObjectData);
	}
}
