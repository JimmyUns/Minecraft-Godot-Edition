using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Inventory_Manager : Node
{
	public int focusedHotbar;
	[Export] public Player_Manager playerManager;
	private bool isHotbarSelectionChanged = true;

	private InventorySlot[,] _inventorySlots = new InventorySlot[10, 4];

	public override void _Ready()
	{
		focusedHotbar = 1;
		
		GiveOject(Block_Manager.Instance.Dirt, 1);
		GiveOject(Block_Manager.Instance.Grass, 1);
		GiveOject(Block_Manager.Instance.Stone, 1);

		//Setting the hotbar icons
		for (int i = 1; i < 10; i++)
		{
			if (_inventorySlots[i, 0] == null) continue;
			playerManager.uiManager.FillHotbarIcons(_inventorySlots[i, 0].Block.Texture_Top, i);
		}
	}

	public override void _Process(double delta)
	{
		//Hotbar Input
		for (int i = 1; i < 10; i++)
		{
			if (Input.IsActionJustPressed("hotbar_" + i))
			{
				focusedHotbar = i;
				isHotbarSelectionChanged = true;
			}
		}
		if (Input.IsActionJustPressed("hotbar_up"))
		{
			focusedHotbar--;
			isHotbarSelectionChanged = true;
		}
		else if (Input.IsActionJustPressed("hotbar_down"))
		{
			focusedHotbar++;
			isHotbarSelectionChanged = true;
		}

		if (focusedHotbar > 9) focusedHotbar = 1;
		else if (focusedHotbar < 1) focusedHotbar = 9;

		//Temporary
		//when inventory system is built, simply change the if with a if(hotbar 0 is selected)
		//then the heldObjectdata is the item in that hotbar data (as in x=0 y=0 is dirt by example)
		//and update hotbar
		if (isHotbarSelectionChanged)
		{
			//Update Hotbar Object Mesh
			isHotbarSelectionChanged = false;
			if (_inventorySlots[focusedHotbar, 0] == null)
			{
				ChangeHeldItem(Block_Manager.Instance.Air);
				playerManager.heldObjectMaker.HandMesh();
			}
			else
			{
				ChangeHeldItem(_inventorySlots[focusedHotbar, 0].Block);
			}
			
			//Update Hotbar Object Name
			if (_inventorySlots[focusedHotbar, 0] == null)
			{
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, "");
			}
			else
			{
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, _inventorySlots[focusedHotbar, 0].Block.name);
			}
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
	
	public void GiveOject(Block block, int Amount)
	{
		for (int i = 1; i < 10; i++)
		{
			if(_inventorySlots[i, 0] == null) 
			{
				_inventorySlots[i, 0] = new InventorySlot(block, Amount);
				return;
			}
		}
	}
}
