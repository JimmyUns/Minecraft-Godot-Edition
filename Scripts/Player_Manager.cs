using Godot;
using System;
using System.Diagnostics;

public partial class Player_Manager : Node3D
{
	[Export] public PlayerBody playerBody;
	[Export] public Camera_Controller cameraController;
	[Export] public Node3D playerMesh;
	[Export] public UI_Manager uiManager;
	[Export] public Actions_Manager actionsManager;
	[Export] public Inventory_Manager inventoryManager;
	[Export] public Voxel_Mesh_Creator vmCreator;

	[Export] public Node3D chunkOutline;


	public int gameMode = 0;
	public int perspectiveMode = 0; //0=fps, 1=tps back view, 2=tps front view
	public bool guiVisible = true;

	public override void _Ready()
	{
		playerMesh.Visible = false;
	}

	public override void _Process(double delta)
	{
		//Perspective Change
		if (Input.IsActionJustPressed("toggle_perspective"))
		{
			perspectiveMode++;
			if (perspectiveMode >= 3)
			{
				perspectiveMode = 0;
				if (guiVisible == true)
					vmCreator.meshLocation.Visible = true;
			}
			else
			{
				if (guiVisible == true)
					vmCreator.meshLocation.Visible = false;
			}
			cameraController.TogglePerspective(perspectiveMode);
		}
		else if (Input.IsActionJustPressed("toggle_gui"))
		{
			guiVisible = !guiVisible;
			if (guiVisible == false) //Hide Gui
			{
				uiManager.Visible = false;
				vmCreator.meshLocation.Visible = false;
			}
			else //Show
			{
				uiManager.Visible = true;
				if (perspectiveMode == 0)
					vmCreator.meshLocation.Visible = true;
			}
		}

		if (Input.IsActionJustPressed("toggle_inventory"))
		{
			if(Input.MouseMode == Input.MouseModeEnum.Captured)
				Input.MouseMode = Input.MouseModeEnum.Visible;
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		
		if (Input.IsActionJustPressed("toggle_gamemode"))
		{
			gameMode++;
			if(gameMode >= 2) gameMode = 0;
		}

		//Debug.Print(GetNode<World_Manager>("%World Manager").chunk);
		Vector3 newPoss = new Vector3(
				(int)Math.Floor(GetCoordinatesGround().X / 16),
				(int)Math.Floor(GetCoordinatesGround().Y / 16),
				(int)Math.Floor(GetCoordinatesGround().Z / 16)
			);

		if (newPoss != chunkOutline.GlobalPosition)
			chunkOutline.GlobalPosition = newPoss * 16;

	}

	public Vector3 GetCoordinatesGround()
	{
		Vector3 flooredPosition = new Vector3(
				(int)Math.Floor(playerBody.GlobalPosition.X),
				(int)Math.Floor(playerBody.GlobalPosition.Y + 1),
				(int)Math.Floor(playerBody.GlobalPosition.Z)
			);
		return flooredPosition;
	}

	public Vector3 GetCoordinatesHead()
	{
		Vector3 flooredPosition = new Vector3(
				(int)Math.Floor(playerBody.GlobalPosition.X),
				(int)Math.Floor(playerBody.GlobalPosition.Y + 2),
				(int)Math.Floor(playerBody.GlobalPosition.Z)
			);
		return flooredPosition;
	}

}
