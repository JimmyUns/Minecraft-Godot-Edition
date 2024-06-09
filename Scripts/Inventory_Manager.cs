using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Inventory_Manager : Node
{
	[Export] public Player_Manager playerManager;
	private bool isHotbarSelectionChanged = false;
	
	private InventorySlot[,] _inventorySlots = new InventorySlot[10, 5];

	public override void _Ready()
	{
		isHotbarSelectionChanged = true;
		playerManager.uiManager.currentHotbarSelected = 1;
		
		_inventorySlots[0, 0] = new InventorySlot(Block_Manager.Instance.Dirt, 1);
		_inventorySlots[1, 0] = new InventorySlot(Block_Manager.Instance.Grass, 1);
		_inventorySlots[8, 0] = new InventorySlot(Block_Manager.Instance.Stone, 1);
		
		for (int i = 0; i < 9; i++)
		{
			if(_inventorySlots[i, 0] == null) continue;
			playerManager.uiManager.FillHotbarIcons(_inventorySlots[i, 0].Block.Texture_Top, i);
			
		}

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
			if(_inventorySlots[playerManager.uiManager.currentHotbarSelected - 1, 0] == null)
			{
				ChangeHeldItem(Block_Manager.Instance.Air);
				playerManager.heldObjectMaker.HandMesh();
				return;
			}
			
			ChangeHeldItem(_inventorySlots[playerManager.uiManager.currentHotbarSelected - 1, 0].Block);
		}
	}

	public void ChangeHeldItem(Block block)
	{
		playerManager.heldObjectMaker.CreateBlockMesh(block);
		playerManager.blockInHand = block;
	}
	
	public class InventorySlot
{
	public Block Block { get; set; }
	public int Amount { get; set; }

	public InventorySlot(Block block, int amount)
	{
		Block = block;
		Amount = amount;
	}
}
}
