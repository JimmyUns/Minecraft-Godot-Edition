using Godot;
using System;
using System.Diagnostics;

public partial class World_Manager : Node
{
	[Export] public PackedScene playerScene, mainmenuScene;
	[Export] public Chunk_World_Manager chunkworldManager;
	
	public static World_Manager instance { get; private set; }
	
	public void StartGame()
	{
		instance = this;
	}
}
