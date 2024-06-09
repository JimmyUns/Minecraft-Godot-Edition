using Godot;
using System;
using System.Diagnostics;

public partial class Actions_Manager : Node
{
	[Export] public ObjectsData heldObjectData;
	[Export] private PackedScene emptyBlock;
	[Export] private RayCast3D playerRaycast, blockbreakRaycast;
	[Export] private Node3D block_outline_node;
	[Export] private Player_Manager playerManager;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		playerRaycast.GlobalRotationDegrees = new Vector3(playerManager.cameraController.playerHead.GlobalRotationDegrees.X, playerRaycast.GlobalRotationDegrees.Y, playerRaycast.GlobalRotationDegrees.Z);

		if (playerRaycast.IsColliding() && playerRaycast.GetCollider() is Chunk_Manager chunk)
		{
			var bPos = playerRaycast.GetCollisionPoint() - 0.5f * playerRaycast.GetCollisionNormal();
			var intbPos = new Vector3(Mathf.FloorToInt(bPos.X), Mathf.FloorToInt(bPos.Y), Mathf.FloorToInt(bPos.Z));

			block_outline_node.Visible = true;
			Vector3 block_outline_node_newPos = intbPos + new Vector3(0.5f, 0.5f, 0.5f);
			if(block_outline_node.GlobalPosition != block_outline_node_newPos)
				block_outline_node.GlobalPosition = block_outline_node_newPos;
				
			if(Input.IsActionJustPressed("action_0")) //break
			{
				chunk.SetBlock((Vector3I)(intbPos - chunk.GlobalPosition), Block_Manager.Instance.Air);
			}

			if(Input.IsActionJustPressed("action_1")) //place
			{
				var placeblockPos = (Vector3I)(intbPos + playerRaycast.GetCollisionNormal());
				if(placeblockPos == playerManager.GetCoordinatesGround() || placeblockPos == playerManager.GetCoordinatesHead()) return; 
				
				Chunk_World_Manager.instance.SetBlock(placeblockPos, playerManager.blockInHand);
			}
		}
		else
		{
			block_outline_node.Visible = false;
		}
	}

}
