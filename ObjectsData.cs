using Godot;
using System;

[Tool]
[GlobalClass]
public partial class ObjectsData : Resource
{
	public ObjectsData() { }
	[Export] public int object_type; //0 is hand, 1 is block, 2 is items, 3 is weapons...
	[Export] public string object_name;
	[Export] public int object_id;
	[Export] public bool object_is_block;
	[Export] public Texture2D texture_top, texture_bottom, texture_front, texture_back, texture_right, texture_left;
	[Export] public Vector3 object_held_position;
	[Export] public Vector3 object_held_rotation;
	
	
	
}
