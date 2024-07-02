using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
	[Export] public Paperdoll_Controller paperdollController;




	[Export] public Node3D chunkOutline;

	public Block blockInHand;
	public int gameMode = 0;
	public int perspectiveMode = 0; //0=fps, 1=tps back view, 2=tps front view
	public bool guiVisible = true;
	public bool inventoryVisible = false, pausemenuVisible;
	private bool debugscreenExtra;

	public override void _Ready()
	{
		lowerbodyMesh.Visible = false;
	}

	public override void _Process(double delta)
	{
		//Perspective Change
		if (Input.IsActionJustPressed("toggle_perspective"))
		{
			TogglePerspective();
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
			if (pausemenuVisible) return;
			inventoryVisible = !inventoryVisible;
			uiManager.ToggleInventory(inventoryVisible);
			LockMouse(!inventoryVisible);
			PausePlayerInput(inventoryVisible);
		}
		else if (Input.IsActionJustPressed("toggle_pause_menu"))
		{
			if (inventoryVisible)
			{
				inventoryVisible = false;
				LockMouse(false);
				PausePlayerInput(false);
				uiManager.ToggleInventory(false);
				return;
			}
			pausemenuVisible = !pausemenuVisible;
			uiManager.pauseMenu.Visible = pausemenuVisible;
			PausePlayerInput(pausemenuVisible);

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


	}

	public Vector3 GetCoordinatesGround()
	{
		return new Vector3I(Mathf.FloorToInt(playerBody.GlobalPosition.X), Mathf.FloorToInt(playerBody.GlobalPosition.Y + 0.3f), Mathf.FloorToInt(playerBody.GlobalPosition.Z));
	}

	public Vector3 GetCoordinatesHead()
	{
		return new Vector3I(Mathf.FloorToInt(playerBody.GlobalPosition.X), Mathf.FloorToInt(playerBody.GlobalPosition.Y + 1.3f), Mathf.FloorToInt(playerBody.GlobalPosition.Z));

	}

	public Vector2I GetChunkPosition()
	{
		return new Vector2I(Mathf.FloorToInt(playerBody.GlobalPosition.X / 16), Mathf.FloorToInt(playerBody.GlobalPosition.Z / 16));
	}

	private async void TogglePerspective()
	{
		perspectiveMode++;
		if (perspectiveMode >= 3)
		{
			perspectiveMode = 0;
			if (guiVisible == true)
			{
				heldObjectMaker.meshInstance.Visible = true;
				uiManager.crosshair.Visible = true;
			}

		}
		else
		{
			if (guiVisible == true)
			{
				heldObjectMaker.meshInstance.Visible = false;
				uiManager.crosshair.Visible = false;

			}
		}
		await Task.Delay(10);
		cameraController.TogglePerspective(perspectiveMode);
	}

	public void LockMouse(bool state)
	{
		if (state)
			Input.MouseMode = Input.MouseModeEnum.Captured;
		else
			Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public void PausePlayerInput(bool state)
	{
		LockMouse(!state);
		playerBody.lockMovement = state;
		cameraController.lockRotation = state;
	}

}
