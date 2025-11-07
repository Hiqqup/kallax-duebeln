using Godot;
using System;
using System.Diagnostics;
using Godot.Collections;
using LetHimCook.scripts;

public partial class I_GraphNode : Node2D
{
	[Export]
	public Array<GraphPath> Paths { get; set; }
	[Export]
	public ProductionResource Input { get; set; }
	[Export]
	public ProductionResource Output { get; set; }
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

		if (Input == ProductionResource.None)
		{
			GD.Print("This node is a root node");
			ProduceOutput();
		}
	}

	public void ProduceOutput()
	{
		if (Output != ProductionResource.None)
		{
			foreach (var graphPath in Paths)
			{
				graphPath.Transport(Output);
			}
		}
	}
	
	public void ReceiveInput(ProductionResource input)
	{
		GD.Print(input.ToString() + " received");
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
