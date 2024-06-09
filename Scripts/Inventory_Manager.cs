using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Inventory_Manager : Node
{
	[Export] public ObjectsData[] ObjectsArchive;
	[Export] public Player_Manager playerManager;

	private bool isHotbarSelectionChanged = false;

	public override void _Ready()
	{
		isHotbarSelectionChanged = true;
		playerManager.uiManager.currentHotbarSelected = 1;
	}

	public override void _Process(double delta)
	{
		//Hotbar Input
		for (int i = 1; i <= 9; i++)
		{
			if (Input.IsActionJustPressed("hotbar_" + i))
			{
				playerManager.uiManager.ChangeSelectedHotbar(i);
				isHotbarSelectionChanged = true;
			}
		}
		if (Input.IsActionJustPressed("hotbar_up"))
		{
			playerManager.uiManager.currentHotbarSelected++;
			playerManager.uiManager.ChangeSelectedHotbar(playerManager.uiManager.currentHotbarSelected);
			isHotbarSelectionChanged = true;
		}
		else if (Input.IsActionJustPressed("hotbar_down"))
		{
			playerManager.uiManager.currentHotbarSelected--;
			playerManager.uiManager.ChangeSelectedHotbar(playerManager.uiManager.currentHotbarSelected);
			isHotbarSelectionChanged = true;
		}

		//Temporary
		//when inventory system is built, simply change the if with a if(hotbar 0 is selected)
		//then the heldObjectdata is the item in that hotbar data (as in x=0 y=0 is dirt by example)
		//and update hotbar
		if (isHotbarSelectionChanged)
		{
			isHotbarSelectionChanged = false;
			if (playerManager.uiManager.currentHotbarSelected == 1) //dirt
			{
				ChangeHeldItem(3);
			}
			else if (playerManager.uiManager.currentHotbarSelected == 2) //grass
			{
				ChangeHeldItem(2);
			}
			else if (playerManager.uiManager.currentHotbarSelected == 3) //stick
			{
				ChangeHeldItem(280);
			}
			else //hand
			{
				playerManager.vmCreator.RemoveMesh();
				playerManager.actionsManager.heldObjectData = playerManager.inventoryManager.ObjectsArchive[playerManager.inventoryManager.GetIndexOfObjectWithId(-1)];
			}
		}
	}

	public void ChangeHeldItem(int objectID)
	{
		playerManager.vmCreator.RemoveMesh();
		playerManager.actionsManager.heldObjectData = playerManager.inventoryManager.ObjectsArchive[playerManager.inventoryManager.GetIndexOfObjectWithId(objectID)];
		playerManager.vmCreator.GenerateVoxelMesh(playerManager.inventoryManager.ObjectsArchive[playerManager.inventoryManager.GetIndexOfObjectWithId(objectID)]);
	}

	public int GetIndexOfObjectWithId(int id)
	{
		for (int i = 0; i <= ObjectsArchive.Length; i++)
		{
			if (ObjectsArchive[i].object_id == id)
			{
				return i;
			}
		}
		Debug.Print("Error, Object with this ID was not found");
		return -2;
	}
}
