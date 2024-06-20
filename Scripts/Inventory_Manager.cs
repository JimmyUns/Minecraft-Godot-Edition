using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Inventory_Manager : Node
{
	public int focusedHotbar = 0;
	[Export] public Player_Manager playerManager;
	public bool isHotbarSelectionChanged = true;

	public InventorySlot[,] _inventorySlots = new InventorySlot[10, 4];
	public InventorySlot heldInventoryObject;

	public override void _Ready()
	{

		GiveOject(Block_Manager.Instance.Dirt, 1);
		GiveOject(Block_Manager.Instance.Grass, 1);
		GiveOject(Block_Manager.Instance.Stone, 1);
		GiveOject(Block_Manager.Instance.Deepslate, 1);
		GiveOject(Block_Manager.Instance.Bedrock, 1);

	}

	public override void _Process(double delta)
	{
		// Hotbar Input
		for (int i = 1; i < 10; i++)
		{
			if (Input.IsActionJustPressed("hotbar_" + i))
			{
				focusedHotbar = i - 1; // Adjust to zero-based index
				isHotbarSelectionChanged = true;
			}
		}

		if (Input.IsActionJustPressed("hotbar_up"))
		{
			focusedHotbar -= 1;
			isHotbarSelectionChanged = true;
		}
		else if (Input.IsActionJustPressed("hotbar_down"))
		{
			focusedHotbar += 1;
			isHotbarSelectionChanged = true;
		}

		// Ensure focusedHotbar stays within valid range
		if (focusedHotbar > 8) focusedHotbar = 0; // Zero-based index for slot 9
		else if (focusedHotbar < 0) focusedHotbar = 8; // Zero-based index for slot 1

		if (isHotbarSelectionChanged)
		{
			isHotbarSelectionChanged = false;
			var currentSlot = _inventorySlots[focusedHotbar, 0];

			if (currentSlot == null || currentSlot.Block == Block_Manager.Instance.Air)
			{
				ChangeHeldItem(Block_Manager.Instance.Air);
				playerManager.heldObjectMaker.HandMesh();
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, "");
			}
			else
			{
				ChangeHeldItem(currentSlot.Block);
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, currentSlot.Block.name);
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
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 9; x++)
			{

				if (_inventorySlots[x, y] == null)
				{
					_inventorySlots[x, y] = new InventorySlot(block, Amount);
					playerManager.uiManager.UpdateInventory();
					return;
				}
			}
		}
	}

	public void GiveOject(Block block, int Amount, int x, int y)
	{
		if (_inventorySlots[x, y] == null)
		{
			_inventorySlots[x, y] = new InventorySlot(block, Amount);
			playerManager.uiManager.UpdateInventory();
			return;
		}
	}
}
