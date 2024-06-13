using Godot;
using System;
using System.Diagnostics;

public partial class Player_Manager : Node3D
{
	[Export] public PlayerBody playerBody;
	[Export] public Camera_Controller cameraController;
	[Export] public Node3D lowerbodyMesh;
	[Export] public UI_Manager uiManager;
	[Export] public Actions_Manager actionsManager;
	[Export] public Inventory_Manager inventoryManager;
	[Export] public Held_Object_Maker heldObjectMaker;
	[Export] public Player_Mesh_Maker bodyMeshMaker;
	[Export] public AnimationTree bodyAnimTree;
	
	

	[Export] public Node3D chunkOutline;

	public Block blockInHand;
	public int gameMode = 0;
	public int perspectiveMode = 0; //0=fps, 1=tps back view, 2=tps front view
	public bool guiVisible = true;
	private bool debugscreenExtra;
	private bool isFullScreen = false;

	public override void _Ready()
	{
		lowerbodyMesh.Visible = false;
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
					heldObjectMaker.meshInstance.Visible = true;

			}
			else
			{
				if (guiVisible == true)
					heldObjectMaker.meshInstance.Visible = false;
			}
			cameraController.TogglePerspective(perspectiveMode);
		}
		else if (Input.IsActionJustPressed("toggle_gui"))
		{
			guiVisible = !guiVisible;
			if (guiVisible == false) //Hide Gui
			{
				uiManager.Visible = false;
				heldObjectMaker.meshInstance.Visible = false;

			}
			else //Show
			{
				uiManager.Visible = true;
				if (perspectiveMode == 0)
					heldObjectMaker.meshInstance.Visible = true;

			}
		}

		if (Input.IsActionJustPressed("toggle_inventory"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
				Input.MouseMode = Input.MouseModeEnum.Visible;
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
		}


		if (Input.IsActionPressed("toggle_debug_screen") && debugscreenExtra == false)
		{
			if (Input.IsKeyPressed(Key.N)) //Change Gamemode
			{
				debugscreenExtra = true;
				gameMode++;
				if (gameMode >= 2) gameMode = 0;
			}
			else if (Input.IsKeyPressed(Key.G)) //Toggle Chunk Border
			{
				debugscreenExtra = true;
				chunkOutline.Visible = !chunkOutline.Visible;
			}
		}
		else if (Input.IsActionJustReleased("toggle_debug_screen"))
		{
			if (debugscreenExtra)
				debugscreenExtra = false;
			else 
			{
				uiManager.debugScreen.Visible = !uiManager.debugScreen.Visible;
				
			}
		}

		//Debug.Print(GetNode<World_Manager>("%World Manager").chunk);
		Vector3 newPoss = new Vector3(
				(int)Math.Floor(GetCoordinatesGround().X / 16),
				0,
				(int)Math.Floor(GetCoordinatesGround().Z / 16)
			);

		if (newPoss != chunkOutline.GlobalPosition)
			chunkOutline.GlobalPosition = newPoss * 16;


		if(Input.IsActionJustPressed("toggle_windowmode"))
		{
			isFullScreen = !isFullScreen;
			if(isFullScreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			} else {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
		}
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
