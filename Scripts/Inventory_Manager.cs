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
	private Block lastheldBlock;
	private bool lastheldBlockFix = true;

	private float currTime;

	public InventorySlot[,] _inventorySlots = new InventorySlot[10, 4];
	public InventorySlot heldInventoryObject;
	[Export] public AnimationPlayer switchheldAnim;

	public override void _Ready()
	{

		/*GiveObject(Block_Manager.Instance.Dirt, 1);
		GiveObject(Block_Manager.Instance.Grass, 2);
		GiveObject(Block_Manager.Instance.Stone, 3);
		GiveObject(Block_Manager.Instance.Deepslate, 4);
		GiveObject(Block_Manager.Instance.Bedrock, 5);
		*/

	}


	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug"))
		{
			GiveObject(Block_Manager.Instance.Grass, 5, 1, 2);
		}

		// Hotbar Input
		if (playerManager.inventoryVisible == false)
		{
			for (int i = 0; i < 9; i++)
			{
				if (Input.IsActionJustPressed("hotbar_" + (i + 1)))
				{
					focusedHotbar = i; // Adjust to zero-based index
					isHotbarSelectionChanged = true;
				}
			}
		}

		if (Input.IsActionJustPressed("drop"))
		{
			if (playerManager.inventoryVisible == false)
			{
				if (Input.IsKeyPressed(Key.Ctrl))
				{
					RemoveObject(64, focusedHotbar, 0);
				}
				else
				{
					RemoveObject(1, focusedHotbar, 0);
				}
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

			if (currentSlot == null || currentSlot.Block == Block_Manager.Instance.Air) //Hand
			{
				playerManager.blockInHand = null;
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, "");

				playerManager.bodyMeshMaker.armROverride.RotationDegrees = new Vector3(0f, 0f, 0f);//Tps object hand

				if (lastheldBlock != null || lastheldBlockFix == true)
					ChangeHeldObject(null);
			}
			else //Object
			{
				playerManager.blockInHand = currentSlot.Block;
				playerManager.uiManager.ChangeSelectedHotbar(focusedHotbar, currentSlot.Block.name);

				playerManager.bodyMeshMaker.armROverride.RotationDegrees = new Vector3(15f, 0f, 0f); //Tps hand

				if (lastheldBlock != currentSlot.Block)
					ChangeHeldObject(currentSlot.Block);
			}
		}

		if (currTime > 0)
		{
			currTime -= (float)delta;
		}
		else
		{
			if (_inventorySlots[focusedHotbar, 0] == null)
			{
				playerManager.heldObjectMaker.meshOffset.Position = new Vector3(0.575f, -0.555f, -0.85f);
				playerManager.heldObjectMaker.meshOffset.RotationDegrees = new Vector3(-17f, -103f, -117.5f);
				playerManager.heldObjectMaker.SetHandMesh();
			}
			else
			{
				playerManager.heldObjectMaker.meshOffset.Position = new Vector3(0.725f, -1.34f, -2.12f);
				playerManager.heldObjectMaker.meshOffset.RotationDegrees = new Vector3(4.5f, 5f, 0f);
				playerManager.heldObjectMaker.CreateBlockMesh(_inventorySlots[focusedHotbar, 0].Block);
			}

		}
	}

	private void ChangeHeldObject(Block block)
	{
		lastheldBlockFix = false;
		lastheldBlock = block;

		currTime = 0.125f;

		if (playerManager.heldObjectMaker.switchheldAnim.IsPlaying() == false)
		{
			playerManager.heldObjectMaker.switchheldAnim.Play("change");
		}
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

	public void GiveObject(Block block, int amount)
	{
		var currentAmount = amount;
		Vector2I freeSlot = new Vector2I(-1, 0);

		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 9; x++)
			{
				if (_inventorySlots[x, y] == null && freeSlot.X == -1)
				{
					freeSlot = new Vector2I(x, y); // Remember the first free slot found
				}
				else if (_inventorySlots[x, y]?.Block == block) // Check if the slot has the same block
				{
					if (_inventorySlots[x, y].Amount + currentAmount > 64)
					{
						var amountNeeded = 64 - _inventorySlots[x, y].Amount;
						currentAmount -= amountNeeded;
						_inventorySlots[x, y].Amount = 64; // Max out this slot
						playerManager.uiManager.UpdateInventory();
					}
					else
					{
						_inventorySlots[x, y].Amount += currentAmount; // Add the remaining amount
						currentAmount = 0;
						playerManager.uiManager.UpdateInventory();
						return; // All blocks have been added
					}
				}
			}
		}

		if (currentAmount > 0)
		{
			if (freeSlot.X > -1)
			{
				_inventorySlots[freeSlot.X, freeSlot.Y] = new InventorySlot(block, currentAmount);
				playerManager.uiManager.UpdateInventory();
			}
			else
			{
				//Drop block cause no empty spot in invetory
			}
		}
	}


	public void GiveObject(Block block, int Amount, int x, int y)
	{
		if (_inventorySlots[x, y] == null)
		{
			_inventorySlots[x, y] = new InventorySlot(block, Amount);
			playerManager.uiManager.UpdateInventory();
			return;
		}
	}

	public void RemoveObject(int Amount, int x, int y)
	{
		_inventorySlots[x, y] = new InventorySlot(_inventorySlots[x, y].Block, _inventorySlots[x, y].Amount - Amount);
		if (_inventorySlots[x, y].Amount <= 0)
		{
			_inventorySlots[x, y] = null;
			playerManager.uiManager.inventoryIcons[x, y].Texture = null;
			playerManager.uiManager.hotbarIcons[x].Texture = null;
			(playerManager.uiManager.hotbarIcons[x].GetNode("amount") as Label).Text = "";
		}

		playerManager.uiManager.UpdateInventory();
	}

	public void GiveObjectHotbar(Block block, int amount)
	{
		var currentAmount = amount;
		Vector2I freeSlot = new Vector2I(-1, 0);

		for (int x = 0; x < 9; x++)
		{
			if (_inventorySlots[x, 0] == null && freeSlot.X == -1)
			{
				freeSlot = new Vector2I(x, 0); // Remember the first free slot found
			}
			else if (_inventorySlots[x, 0]?.Block == block) // Check if the slot has the same block
			{
				if (_inventorySlots[x, 0].Amount + currentAmount > 64)
				{
					var amountNeeded = 64 - _inventorySlots[x, 0].Amount;
					currentAmount -= amountNeeded;
					_inventorySlots[x, 0].Amount = 64; // Max out this slot
					playerManager.uiManager.UpdateInventory();
				}
				else
				{
					_inventorySlots[x, 0].Amount += currentAmount; // Add the remaining amount
					currentAmount = 0;
					playerManager.uiManager.UpdateInventory();
					return; // All blocks have been added
				}
			}
		}


		if (currentAmount > 0)
		{
			if (freeSlot.X > -1)
			{
				_inventorySlots[freeSlot.X, freeSlot.Y] = new InventorySlot(block, currentAmount);
				playerManager.uiManager.UpdateInventory();
			}
			else
			{
				//Drop block cause no empty spot in invetory
			}
		}
	}

	public void GiveObjectNotHotbar(Block block, int amount)
	{
		var currentAmount = amount;
		Vector2I freeSlot = new Vector2I(-1, 0);

		for (int y = 1; y < 4; y++)
		{
			for (int x = 0; x < 9; x++)
			{
				if (_inventorySlots[x, y] == null && freeSlot.X == -1)
				{
					freeSlot = new Vector2I(x, y); // Remember the first free slot found
				}
				else if (_inventorySlots[x, y]?.Block == block) // Check if the slot has the same block
				{
					if (_inventorySlots[x, y].Amount + currentAmount > 64)
					{
						var amountNeeded = 64 - _inventorySlots[x, y].Amount;
						currentAmount -= amountNeeded;
						_inventorySlots[x, y].Amount = 64; // Max out this slot
						playerManager.uiManager.UpdateInventory();
					}
					else
					{
						_inventorySlots[x, y].Amount += currentAmount; // Add the remaining amount
						currentAmount = 0;
						playerManager.uiManager.UpdateInventory();
						return; // All blocks have been added
					}
				}
			}
		}

		if (currentAmount > 0)
		{
			if (freeSlot.X > -1)
			{
				_inventorySlots[freeSlot.X, freeSlot.Y] = new InventorySlot(block, currentAmount);
				playerManager.uiManager.UpdateInventory();
			}
			else
			{
				//Drop block cause no empty spot in invetory
			}
		}
	}
}
