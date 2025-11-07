using Godot;
using System;
using System.Diagnostics;
using Godot.Collections;

public partial class I_GraphNode : Node2D
{
	[Export]
	public Array<GraphPath> Paths { get; set; }
	[Export]
	public string Input { get; set; }
	[Export]
	public string Output { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Paths != null)
		{
			foreach (var graphPaths in Paths)
			{
				graphPaths.ParentNode = this;
				GD.Print("set parent node for path");
			}
		}

		if (Input == null || Input == "")
		{
			GD.Print("This node is a root node");
			ProduceOutput();
		}
	}

	public void ProduceOutput()
	{
		if (Output != null || Output != "")
		{
			foreach (var graphPath in Paths)
			{
				graphPath.Transport(Output);
			}
		}
	}
	
	public void ReceiveInput(string input)
	{
		GD.Print(input + " received");
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
