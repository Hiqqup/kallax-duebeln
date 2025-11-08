using Godot;
using System;
using Godot.Collections;

public partial class SpriteLookup : Node
{
	public static System.Collections.Generic.Dictionary<ProductionResource, string> lookup = new ()
	{
		{ ProductionResource.A, "res://assets/parts/screw_plug.png"},
	};

	public static string MapResourceToFile(ProductionResource resource)
	{
		return (lookup.TryGetValue(resource, out var value)) ? value : "res://assets/placeholder/breathtaking_illustration_of_s.jpeg";
	}
}
