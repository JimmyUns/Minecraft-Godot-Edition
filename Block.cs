using Godot;
using System;

[Tool]
[GlobalClass]
public partial class Block : Resource
{
	public Block() { }
	[Export] public String name;
	[Export] public Texture2D Texture_Top, Texture_Bottom, Texture_Right, Texture_Left, Texture_Forward, Texture_Back;
	public Texture2D[] Textures => new Texture2D[] { Texture_Top, Texture_Bottom, Texture_Right, Texture_Left, Texture_Forward, Texture_Back };
	[Export] public bool isTransparent;

}
