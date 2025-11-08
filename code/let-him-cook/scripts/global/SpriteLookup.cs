using Godot;
using System;
using Godot.Collections;

public partial class SpriteLookup : Node
{
	public Dictionary lookup = new Godot.Collections.Dictionary
	{
		{(int) ProductionResource.A, "res://assets/placeholder/Bild1-2.png"},
	};
}
