using Godot;
using System;
using Godot.Collections;

public partial class LevelGenerator : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = GetViewport().GetCamera2D().GetGlobalPosition();
		
		for (var i = 0; i < 500; i++)
		{
			var inputs = new Array<ResourceAmount>();
			var outputs = new Array<ResourceAmount>();
			// create a producer
			var randomResource = GetRandomProductionResource(i);
			inputs.Add(new ResourceAmount(randomResource, GD.RandRange((int)Math.Floor(i*2.5f),(int)Math.Floor(i*4.5f))));
			
			var spawnDirection = Vector2.FromAngle(GD.RandRange(0,360));
			var spawnDistance = 1500 * Math.Sqrt(0.05f * i);
			
			var rewardNode = GD.Load<PackedScene>("res://scenes/graph_nodes/node.tscn");
			var createdNode = rewardNode.Instantiate() as I_GraphNode;

			if (createdNode != null)
			{
				var targetPos = Position + spawnDirection * (float)spawnDistance;
				if (targetPos == GetViewport().GetCamera2D().GetGlobalPosition()) targetPos = Vector2.One;
				createdNode.SetPosition(targetPos);
                createdNode.Recource_Input = inputs;
                createdNode.Output = outputs;
                
                GetTree().GetRoot().AddChild(createdNode);
			}
		}
		CreateInitialDowelProducer();
	}

	private ProductionResource GetRandomProductionResource(int index)
	{
		if (index < 100)
		{
			return ProductionResourceExtensions.GetRandomT1Resource();
		}
		else if (index < 200)
		{
			return ProductionResourceExtensions.GetRandomT2Resource();
		}
		else if (index < 300)
		{
			return ProductionResourceExtensions.GetRandomT3Resource();
		}
		else
		{
			return ProductionResourceExtensions.GetRandomT4Resource();
		}
	}

	private void CreateInitialDowelProducer()
	{
		var inputs = new Array<ResourceAmount>();
        var outputs = new Array<ResourceAmount>();
        
		var rewardNode = GD.Load<PackedScene>("res://scenes/graph_nodes/node.tscn");
		var createdNode = rewardNode.Instantiate() as I_GraphNode;
        
		if (createdNode != null)
		{
			var randomResource = ProductionResourceExtensions.GetRandom();
			outputs.Add(new ResourceAmount(ProductionResource.Dowel, 2));
			
			createdNode.SetPosition( GetViewport().GetCamera2D().GetGlobalPosition() );
			createdNode.Recource_Input = inputs;
			createdNode.Output = outputs;
		    
			GetTree().GetRoot().AddChild(createdNode);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
