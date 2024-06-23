using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Main_Menu_Manager : Node
{
	[Export] public Camera3D mainCamera;
	[Export] public TextureRect logoscreenTexture;
	[Export] private AnimationPlayer splashtextAnim;
	
	private bool isshowingMenu;
	public override void _Ready()
	{
		FakeLoadingScreenLOL();
	}
	public override void _Process(double delta)
	{
		if (isshowingMenu)
		{
			mainCamera.RotateY(-((float)delta * 0.1f));

			if (logoscreenTexture.Modulate.A > 0)
			{
				logoscreenTexture.Modulate = new Color(1, 1, 1, logoscreenTexture.Modulate.A - (float)delta);
			}
			else
			{
				logoscreenTexture.MouseFilter = Control.MouseFilterEnum.Ignore;
			}
		}
	}

	public async void FakeLoadingScreenLOL()
	{
		logoscreenTexture.Visible = true;
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
		await ToSignal(GetTree().CreateTimer(5), "timeout");
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
		isshowingMenu = true;
			splashtextAnim.Play("new_animation");
		
	}

	public void _on_singleplayer_button_pressed()
	{
		Game_manager.instance.Start_World();
	}

	public void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}
}
