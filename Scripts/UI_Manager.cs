using Godot;
using System;
using System.Diagnostics;

public partial class UI_Manager : CanvasLayer
{
	[Export] public Player_Manager playerManager;
	[Export] public Label coordinates, chunk, fps, hotbarObjectNamePopup;
	[Export] private TextureRect hotbarSelected_TextureRect;
	[Export] public TextureRect inventory_survival, hotbar;
	public TextureRect[,] inventoryIcons = new TextureRect[10, 4];

	[Export] public TextureRect[] hotbarIcons;
	[Export] public AnimationPlayer ui_Anim;
	[Export] public CanvasLayer debugScreen;
	[Export] public TextureRect crosshair, slotHighlight, heldInventoryObjectTextureRect;


	public override void _Ready()
	{
		var containerNode1 = inventory_survival.GetNode("Container_Hotbar");
		var containerNode2 = inventory_survival.GetNode("Container");


		for (int i = 0; i < containerNode1.GetChildCount(); i++)
		{
			var textureRect = containerNode1.GetChild(i) as TextureRect;
			if (textureRect != null)
			{
				inventoryIcons[i % 9, 0] = textureRect;
			}
		}

		int currY = 1;

		for (int i = 0; i < containerNode2.GetChildCount(); i++)
		{
			if (i == 9 || i == 18)
			{
				currY++;
			}
			var textureRect = containerNode2.GetChild(i) as TextureRect;
			if (textureRect != null)
			{
				inventoryIcons[i % 9, currY] = textureRect;
			}
		}


		hotbarObjectNamePopup.SelfModulate = new Color(1f, 1f, 1f, 1f);
		inventory_survival.Visible = false;
	}

	public override void _Process(double delta)
	{
		coordinates.Text = "X:" + playerManager.GetCoordinatesGround().X + ", Y:" + playerManager.GetCoordinatesGround().Y + ", Z:" + playerManager.GetCoordinatesGround().Z;
		chunk.Text = "Chunk: " + playerManager.GetChunkPosition().ToString();
		fps.Text = "fps: " + Engine.GetFramesPerSecond().ToString();

		InventoryActions();
	}

	public void ChangeSelectedHotbar(int index, string objectName)
	{
		hotbarObjectNamePopup.Text = objectName;
		ui_Anim.Stop();
		ui_Anim.Play("ObjectName_Hide");

		playerManager.inventoryManager.focusedHotbar = index;
		switch (index)
		{
			case 0:
				hotbarSelected_TextureRect.SetPosition(new Vector2(-2f, -2.5f));
				break;
			case 1:
				hotbarSelected_TextureRect.SetPosition(new Vector2(48.5f, -2.5f));
				break;
			case 2:
				hotbarSelected_TextureRect.SetPosition(new Vector2(100.5f, -2.5f));
				break;
			case 3:
				hotbarSelected_TextureRect.SetPosition(new Vector2(152.5f, -2.5f));
				break;
			case 4:
				hotbarSelected_TextureRect.SetPosition(new Vector2(204.5f, -2.5f));
				break;
			case 5:
				hotbarSelected_TextureRect.SetPosition(new Vector2(256.5f, -2.5f));
				break;
			case 6:
				hotbarSelected_TextureRect.SetPosition(new Vector2(308.5f, -2.5f));
				break;
			case 7:
				hotbarSelected_TextureRect.SetPosition(new Vector2(359.5f, -2.5f));
				break;
			case 8:
				hotbarSelected_TextureRect.SetPosition(new Vector2(411.5f, -2.5f));
				break;
		}
	}

	public void UpdateInventory()
	{
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 9; x++)
			{
				var inventorySlot = playerManager.inventoryManager._inventorySlots[x, y];
				if (inventorySlot != null)
				{
					var texture = inventorySlot.Block.Texture_Top;
					inventoryIcons[x, y].Texture = texture;

					if (inventorySlot.Amount == 0 || inventorySlot.Amount == 1)
					{
						(inventoryIcons[x, y].GetNode("amount") as Label).Text = "";
					}
					else
					{
						(inventoryIcons[x, y].GetNode("amount") as Label).Text = inventorySlot.Amount.ToString();
					}

					if (y == 0)
					{
						FillHotbarIcons(texture, inventorySlot.Amount, x);
					}
				}
			}
		}
		playerManager.inventoryManager.isHotbarSelectionChanged = true;
	}

	private void FillHotbarIcons(Texture2D texture, int amount, int index)
	{
		if (index >= 0 && index < hotbarIcons.Length)
		{
			hotbarIcons[index].Texture = texture;
			if (amount == 0 || amount == 1)
			{
				(hotbarIcons[index].GetNode("amount") as Label).Text = "";
			}
			else
			{
				(hotbarIcons[index].GetNode("amount") as Label).Text = amount.ToString();
			}
		}
	}


	public void ToggleInventory(bool state)
	{
		hotbar.Visible = !state;
		inventory_survival.Visible = state;
	}

	public void OnMouseEnteredIcon(TextureRect textureRect)
	{
		GD.Print($"Mouse entered {textureRect.Name}");
	}

	public void OnMouseEdIcon(TextureRect textureRect)
	{
		GD.Print($"Mouse exited {textureRect.Name}");
	}

	private void ChangeHeldObjectValue()
	{
		if (playerManager.inventoryManager.heldInventoryObject == null) return;
		var value = playerManager.inventoryManager.heldInventoryObject.Amount;
		if (value == 1)
		{
			(heldInventoryObjectTextureRect.GetNode("amount") as Label).Text = "";
			return;
		}
		(heldInventoryObjectTextureRect.GetNode("amount") as Label).Text = value.ToString();
	}

	private void InventoryActions()
	{
		if (playerManager.inventoryVisible)
		{
			Vector2 mousePosition = GetViewport().GetMousePosition();
			bool isMouseOverSlot = false;

			foreach (TextureRect textureRect in inventoryIcons)
			{
				if (textureRect != null && textureRect.GetGlobalRect().HasPoint(mousePosition)) //If mouse is on slot
				{
					slotHighlight.Visible = true;
					slotHighlight.GlobalPosition = textureRect.GlobalPosition;
					isMouseOverSlot = true;

					if (Input.IsActionJustPressed("action_0"))
					{
						for (int y = 0; y < 4; y++)
						{
							for (int x = 0; x < 9; x++)
							{
								if (inventoryIcons[x, y] == textureRect) //If the inventory slot the one for the icon, as in find the inventoryIcons related to that textureRect
								{
									if (Input.IsKeyPressed(Key.Shift))
									{
										if (y == 0)
										{
											if (inventoryIcons[x, y].Texture != null)
											{
												var temp = playerManager.inventoryManager._inventorySlots[x, y];
												playerManager.inventoryManager.RemoveObject(64, x, y);
												playerManager.inventoryManager.GiveObjectNotHotbar(temp.Block, temp.Amount);
												(inventoryIcons[x, y].GetNode("amount") as Label).Text = "";
												UpdateInventory();
											}
										}
										else
										{
											if (inventoryIcons[x, y].Texture != null)
											{
												var temp = playerManager.inventoryManager._inventorySlots[x, y];
												playerManager.inventoryManager.RemoveObject(64, x, y);
												playerManager.inventoryManager.GiveObjectHotbar(temp.Block, temp.Amount);
												(inventoryIcons[x, y].GetNode("amount") as Label).Text = "";
												UpdateInventory();
											}
										}

									}
									else
									{
										if (!heldInventoryObjectTextureRect.Visible && inventoryIcons[x, y].Texture != null) // Not holding something
										{
											playerManager.inventoryManager.heldInventoryObject = playerManager.inventoryManager._inventorySlots[x, y]; // Save the selected block and its data
											heldInventoryObjectTextureRect.Texture = playerManager.inventoryManager._inventorySlots[x, y].Block.Texture_Top;
											heldInventoryObjectTextureRect.Visible = true;


											playerManager.inventoryManager.RemoveObject(64, x, y); // Remove from inventory
											textureRect.Texture = Block_Manager.Instance.Air.Texture_Top;
											(inventoryIcons[x, y].GetNode("amount") as Label).Text = "";

											// Updating Hotbar
											if (y == 0)
											{
												FillHotbarIcons(Block_Manager.Instance.Air.Texture_Top, 0, x);
												playerManager.inventoryManager.isHotbarSelectionChanged = true;
											}
										}
										else if (heldInventoryObjectTextureRect.Visible) // Is holding something
										{
											if (playerManager.inventoryManager._inventorySlots[x, y] == null) // No object in that slot
											{
												playerManager.inventoryManager.GiveObject(playerManager.inventoryManager.heldInventoryObject.Block, playerManager.inventoryManager.heldInventoryObject.Amount, x, y);

												playerManager.inventoryManager.heldInventoryObject = null; // Clear held object
												heldInventoryObjectTextureRect.Texture = null;
												heldInventoryObjectTextureRect.Visible = false;
											}
											else // Object found in slot, replace it
											{
												if (playerManager.inventoryManager._inventorySlots[x, y].Block == playerManager.inventoryManager.heldInventoryObject.Block)
												{
													// Add amount from held block to slot
													int totalAmount = playerManager.inventoryManager._inventorySlots[x, y].Amount + playerManager.inventoryManager.heldInventoryObject.Amount;
													if (totalAmount > 64)
													{
														int excessAmount = totalAmount - 64;
														playerManager.inventoryManager._inventorySlots[x, y].Amount = 64;
														playerManager.inventoryManager.heldInventoryObject.Amount = excessAmount;
													}
													else
													{
														playerManager.inventoryManager._inventorySlots[x, y].Amount = totalAmount;
														playerManager.inventoryManager.heldInventoryObject = null;
														heldInventoryObjectTextureRect.Texture = null;
														heldInventoryObjectTextureRect.Visible = false;
													}
												}
												else
												{
													// Swap items
													var _heldInventoryObject = playerManager.inventoryManager.heldInventoryObject; // Save held object in a temp variable for switch

													playerManager.inventoryManager.heldInventoryObject = playerManager.inventoryManager._inventorySlots[x, y];
													heldInventoryObjectTextureRect.Texture = playerManager.inventoryManager.heldInventoryObject.Block.Texture_Top;

													playerManager.inventoryManager.RemoveObject(64, x, y); // Remove old object from inventory
													playerManager.inventoryManager.GiveObject(_heldInventoryObject.Block, _heldInventoryObject.Amount, x, y); // Add held item into inventory

												}
											}
											UpdateInventory();
										}
									}
									ChangeHeldObjectValue();
								}
							}
						}
					}
					else if (Input.IsActionJustPressed("action_1"))
					{
						for (int y = 0; y < 4; y++)
						{
							for (int x = 0; x < 9; x++)
							{
								if (inventoryIcons[x, y] == textureRect) //If the inventory slot the one for the icon, as in find the inventoryIcons related to that textureRect
								{

									if (!heldInventoryObjectTextureRect.Visible && inventoryIcons[x, y].Texture != null && playerManager.inventoryManager._inventorySlots[x, y].Amount > 1) // Not holding something, so cut the amount in half
									{
										int remainingAmount = playerManager.inventoryManager._inventorySlots[x, y].Amount / 2;
										int heldAmount = playerManager.inventoryManager._inventorySlots[x, y].Amount - remainingAmount;

										playerManager.inventoryManager._inventorySlots[x, y].Amount = remainingAmount;
										if (remainingAmount == 1)
										{
											(inventoryIcons[x, y].GetNode("amount") as Label).Text = "";
										}
										else
										{
											(inventoryIcons[x, y].GetNode("amount") as Label).Text = remainingAmount.ToString();
										}

										playerManager.inventoryManager.heldInventoryObject = new Inventory_Manager.InventorySlot(
											playerManager.inventoryManager._inventorySlots[x, y].Block,
											heldAmount
										);

										heldInventoryObjectTextureRect.Texture = playerManager.inventoryManager._inventorySlots[x, y].Block.Texture_Top;
										heldInventoryObjectTextureRect.Visible = true;

										// Updating Hotbar
										if (y == 0)
										{
											FillHotbarIcons(Block_Manager.Instance.Air.Texture_Top, 0, x);
											playerManager.inventoryManager.isHotbarSelectionChanged = true;
										}
									}
									else if (heldInventoryObjectTextureRect.Visible) // Is holding something
									{
										if (playerManager.inventoryManager._inventorySlots[x, y] == null) // No object in that slot
										{
											playerManager.inventoryManager.heldInventoryObject.Amount--;
											playerManager.inventoryManager.GiveObject(playerManager.inventoryManager.heldInventoryObject.Block, 1, x, y);
											if (playerManager.inventoryManager.heldInventoryObject.Amount <= 0)
											{
												playerManager.inventoryManager.heldInventoryObject = null;
												heldInventoryObjectTextureRect.Visible = false;
											}
										}
										else // Object found in slot, add to it
										{
											if (playerManager.inventoryManager._inventorySlots[x, y].Block == playerManager.inventoryManager.heldInventoryObject.Block && playerManager.inventoryManager._inventorySlots[x, y].Amount < 64)
											{
												playerManager.inventoryManager.heldInventoryObject.Amount--;
												var savedAmount = playerManager.inventoryManager._inventorySlots[x, y].Amount + 1;
												playerManager.inventoryManager.RemoveObject(playerManager.inventoryManager._inventorySlots[x, y].Amount, x, y);
												playerManager.inventoryManager.GiveObject(playerManager.inventoryManager.heldInventoryObject.Block, savedAmount, x, y);
												(inventoryIcons[x, y].GetNode("amount") as Label).Text = playerManager.inventoryManager._inventorySlots[x, y].Amount.ToString();
												if (playerManager.inventoryManager.heldInventoryObject.Amount <= 0)
												{
													playerManager.inventoryManager.heldInventoryObject = null;
													heldInventoryObjectTextureRect.Visible = false;
												}
											}
										}
										UpdateInventory();
									}

								}
								ChangeHeldObjectValue();
							}
						}
					}
				}

			}


			if (!isMouseOverSlot)
			{
				slotHighlight.Visible = false; // Hide slot highlight when not hovering over any slot
			}


			if (!isMouseOverSlot || inventory_survival.Visible == false)
			{
				slotHighlight.Visible = false;
			}

			if (playerManager.inventoryManager.heldInventoryObject != null)
			{
				heldInventoryObjectTextureRect.GlobalPosition = mousePosition - new Vector2(24.25f, 24.25f);
			}
			else
			{
				heldInventoryObjectTextureRect.Visible = false;
			}
		}
	}
}
