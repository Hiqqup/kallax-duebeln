using Godot;
using System;
using Godot.Collections;

public partial class SpriteLookup : Node
{
	public static System.Collections.Generic.Dictionary<ProductionResource, string> lookup = new ()
	{
		
		{ ProductionResource.Dowel , "res://assets/parts/dowel.png"},
		{ ProductionResource.PointyScrew , "res://assets/parts/pointy_screw.png"},
		{ ProductionResource.ScrewPlug, "res://assets/parts/screw_plug.png"},
		{ ProductionResource.DoubleEndedScrew , "res://assets/parts/double_ended_screw.png"},
		{ ProductionResource.Hinge , "res://assets/parts/hinge.png"},
		{ ProductionResource.Plank, "res://assets/parts/Plank.png"},
		{ ProductionResource.Kallax1x1 , "res://assets/parts/Kallax1x1.png"},
		{ ProductionResource.Kallax1x2 , "res://assets/parts/Kallax1x2.png"},
		{ ProductionResource.Kallax2x2 , "res://assets/parts/Kallax2x2.png"},
		{ ProductionResource.kallax2x4 , "res://assets/parts/Kallax2x4.png"}
		
	};

	public static string MapResourceToFile(ProductionResource resource)
	{
		return (lookup.TryGetValue(resource, out var value)) ? value : "res://assets/placeholder/breathtaking_illustration_of_s.jpeg";
	}
}
