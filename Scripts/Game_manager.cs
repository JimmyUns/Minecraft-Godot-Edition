using Godot;
using System;
using System.Runtime.InteropServices;

public partial class Game_manager : Node
{
	[Export] private PackedScene MainMenu_Scene;
	[Export] private PackedScene World_Scene;
	public static Game_manager instance { get; private set; }

	private Node3D MainMenu_Node;
	private Node3D World_Node;
	
	private bool firstLaunch = true;

	public override void _Ready()
	{
		Start_Main_Menu();
	}
	public override void _Process(double delta)
	{
		Change_Window_Mode();
	}
	
	public void Start_Main_Menu()
	{
		instance = this;
		MainMenu_Node = MainMenu_Scene.Instantiate<Node3D>();
		AddChild(MainMenu_Node);
	}
	public void Start_World()
	{
		MainMenu_Node.QueueFree();
		World_Node = World_Scene.Instantiate<Node3D>();
		AddChild(World_Node);
	}


	public void Change_Window_Mode()
	{
		if (Input.IsActionJustPressed("toggle_windowmode"))
		{
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			}
			else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
		}
	}
}

