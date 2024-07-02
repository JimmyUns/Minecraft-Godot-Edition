using Godot;
using System;

public partial class Paperdoll_Controller : Node
{
	[Export] public Node3D paperdollHolder;
	[Export] public Player_Manager playerManager;
	[Export] public MeshInstance3D head, body, armL, armR, legL, legR;
	[Export] public MeshInstance3D headOL, bodyOL, armLOL, armROL, legLOL, legROL;

	public override void _Ready()
	{
		paperdollHolder.GlobalPosition = body.GlobalPosition;
   
	}
	public void CreatePaperdoll()
	{
		head.Mesh = playerManager.bodyMeshMaker.head.Mesh;
		body.Mesh = playerManager.bodyMeshMaker.body.Mesh;
		armL.Mesh = playerManager.bodyMeshMaker.armL.Mesh;
		armR.Mesh = playerManager.bodyMeshMaker.armR.Mesh;
		legL.Mesh = playerManager.bodyMeshMaker.legL.Mesh;
		legR.Mesh = playerManager.bodyMeshMaker.legR.Mesh;
		
		headOL.Mesh = playerManager.bodyMeshMaker.headOL.Mesh;
		bodyOL.Mesh = playerManager.bodyMeshMaker.bodyOL.Mesh;
		armLOL.Mesh = playerManager.bodyMeshMaker.armLOL.Mesh;
		armROL.Mesh = playerManager.bodyMeshMaker.armROL.Mesh;
		legLOL.Mesh = playerManager.bodyMeshMaker.legLOL.Mesh;
		legROL.Mesh = playerManager.bodyMeshMaker.legROL.Mesh;
	}
}
