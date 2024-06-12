using Godot;
using System;

public partial class UI_Manager : CanvasLayer
{
	[Export] public Player_Manager playerManager;
	[Export] public Label coordinates, fps, hotbarObjectNamePopup;
	[Export] private TextureRect hotbarSelected_TextureRect;
	[Export] public TextureRect[] hotbarIcons;
	[Export] public AnimationPlayer ui_Anim;
	[Export] public CanvasLayer debugScreen;

	public override void _Ready()
	{
		hotbarObjectNamePopup.SelfModulate = new Color(1f, 1f, 1f, 1f);
	}

	public override void _Process(double delta)
	{
		coordinates.Text = "X:" + playerManager.GetCoordinatesGround().X + ", Y:" + playerManager.GetCoordinatesGround().Y + ", Z:" + playerManager.GetCoordinatesGround().Z;
		fps.Text = "fps: " + Engine.GetFramesPerSecond().ToString();
	}

	public void ChangeSelectedHotbar(int index, string objectName)
	{
		hotbarObjectNamePopup.Text = objectName;
		ui_Anim.Stop();
		ui_Anim.Play("ObjectName_Hide");

		playerManager.inventoryManager.focusedHotbar = index;
		switch (index)
		{
			case 1:
				hotbarSelected_TextureRect.SetPosition(new Vector2(-2f, -2.5f));
				break;
			case 2:
				hotbarSelected_TextureRect.SetPosition(new Vector2(48.5f, -2.5f));
				break;
			case 3:
				hotbarSelected_TextureRect.SetPosition(new Vector2(100.5f, -2.5f));
				break;
			case 4:
				hotbarSelected_TextureRect.SetPosition(new Vector2(152.5f, -2.5f));
				break;
			case 5:
				hotbarSelected_TextureRect.SetPosition(new Vector2(204.5f, -2.5f));
				break;
			case 6:
				hotbarSelected_TextureRect.SetPosition(new Vector2(256.5f, -2.5f));
				break;
			case 7:
				hotbarSelected_TextureRect.SetPosition(new Vector2(308.5f, -2.5f));
				break;
			case 8:
				hotbarSelected_TextureRect.SetPosition(new Vector2(359.5f, -2.5f));
				break;
			case 9:
				hotbarSelected_TextureRect.SetPosition(new Vector2(411.5f, -2.5f));
				break;
		}
	}


	public void FillHotbarIcons(Texture2D texture, int index)
	{
		hotbarIcons[index - 1].Texture = texture;
	}
}
