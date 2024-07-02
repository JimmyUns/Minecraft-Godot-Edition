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
	[Export] public AnimationPlayer handheldobjectAnim;


	private float currBreakCooldown, currPlaceCooldown;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (currBreakCooldown > 0) currBreakCooldown -= (float)delta;
		if (currPlaceCooldown > 0) currPlaceCooldown -= (float)delta;

		playerRaycast.GlobalRotationDegrees = new Vector3(playerManager.cameraController.playerHead.GlobalRotationDegrees.X, playerRaycast.GlobalRotationDegrees.Y, playerRaycast.GlobalRotationDegrees.Z);

		if (playerRaycast.IsColliding() && playerRaycast.GetCollider() is Chunk_Manager chunk)
		{
			var bPos = playerRaycast.GetCollisionPoint() - 0.5f * playerRaycast.GetCollisionNormal();
			var intbPos = new Vector3(Mathf.FloorToInt(bPos.X), Mathf.FloorToInt(bPos.Y), Mathf.FloorToInt(bPos.Z));

			block_outline_node.Visible = true;
			Vector3 block_outline_node_newPos = intbPos + new Vector3(0.5f, 0.5f, 0.5f);
			if (block_outline_node.GlobalPosition != block_outline_node_newPos)
				block_outline_node.GlobalPosition = block_outline_node_newPos;


			if (playerManager.inventoryVisible || playerManager.pausemenuVisible) return;

			if (Input.IsActionPressed("action_0") && currBreakCooldown <= 0) //break
			{

				playerManager.inventoryManager.GiveObject(chunk.GetBlock((Vector3I)(intbPos - chunk.GlobalPosition)), 1);
				chunk.SetBlock((Vector3I)(intbPos - chunk.GlobalPosition), Block_Manager.Instance.Air);
				currBreakCooldown = 0.1f;

				if (handheldobjectAnim.IsPlaying() == false)
				{
					if (playerManager.blockInHand != null)
						handheldobjectAnim.Play("break_block");
					else
						handheldobjectAnim.Play("break_hand");
				}
			}

			if (Input.IsActionPressed("action_1") && currPlaceCooldown <= 0 && playerManager.inventoryVisible == false && playerManager.blockInHand != null) //place
			{
				var placeblockPos = (Vector3I)(intbPos + playerRaycast.GetCollisionNormal());
				if (placeblockPos == playerManager.GetCoordinatesGround() || placeblockPos == playerManager.GetCoordinatesHead()) return;

				Chunk_World_Manager.instance.SetBlock(placeblockPos, playerManager.blockInHand);

				if (playerManager.gameMode != 1)
				{
					playerManager.inventoryManager.RemoveObject(1, playerManager.inventoryManager.focusedHotbar, 0);
					playerManager.inventoryManager.isHotbarSelectionChanged = true;
				}

				if (handheldobjectAnim.IsPlaying() == false)
					handheldobjectAnim.Play("place_block");

				currPlaceCooldown = 0.25f;
			}
			else if (Input.IsActionJustReleased("action_1"))
			{
				currPlaceCooldown = 0;
			}
		}
		else
		{
			block_outline_node.Visible = false;

			if (playerManager.inventoryVisible || playerManager.pausemenuVisible) return;
			//Same as attacking
			if (Input.IsActionJustPressed("action_0"))
			{
				if (handheldobjectAnim.IsPlaying() == false)
				{
					if (playerManager.blockInHand != null)
						handheldobjectAnim.Play("break_block");
					else
						handheldobjectAnim.Play("break_hand");
				}
			}
		}
	}

}
