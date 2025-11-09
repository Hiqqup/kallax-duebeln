using Godot;
using System;
using Godot.Collections;

public partial class Reward : Control
{
	private Action<int> _callback;
	private Array<I_GraphNode> _rewards = new Array<I_GraphNode>();
	// Called when the node enters the scene tree for the first time.
	
	private void GenerateRewards()
	{
		_rewards.Clear();
		for (int i = 0; i < 3; i++)
		{
			var inputs = new Array<ResourceAmount>();
			var outputs = new Array<ResourceAmount>();
			if (i == 0)
			{
				// create a producer
				var randomResource = ProductionResourceExtensions.GetRandom();
				outputs.Add(new ResourceAmount(randomResource, GD.RandRange(1,5)));
			}
			else
			{
				var amountOfInputs = GD.RandRange(1, 3);
				for (int j = 0; j < amountOfInputs; j++)
				{
					
				}
				outputs.Add(new ResourceAmount(ProductionResourceExtensions.GetRandom(), GD.RandRange(1, 5)));
			}
			
			_rewards.Add(new I_GraphNode(inputs, outputs));
		}
	}
	
	public override void _Ready()
	{
		GenerateRewards();
		_callback = CallbackWasntSetWarning;
	}

	private void InstantiateSelectedReward(int selected)
	{
		var selectedNode = _rewards[selected];
		
		var rewardNode = GD.Load<PackedScene>("res://scenes/graph_nodes/node.tscn");
		var createdNode = rewardNode.Instantiate() as I_GraphNode;
		
		// Copy the data from your constructed node to the instantiated one
		createdNode.Recource_Input = selectedNode.Recource_Input;
		createdNode.Output = selectedNode.Output;
        	
		// Now add it to the scene tree - this will trigger _Ready() which will use the data
		GetTree().Root.AddChild(createdNode);
		createdNode.SetPosition(GetViewport().GetCamera2D().GetGlobalPosition());
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartRewardSelection(Action<int> c)
	{
		 Visible = true;
		_callback = c;
	}

	private void CallbackWasntSetWarning(int i)
	{
		GD.Print($"WARNING: CALLBACK WASN'T SET");
	}
	
	private void StopRewardSelection(int i)
	{
		Visible = false;
		InstantiateSelectedReward(i);
		_callback(i);
	}

	public void RewardSelected0()
	{
		StopRewardSelection(0);
	}
	public void RewardSelected1()
	{
		StopRewardSelection(1);
	}
	public void RewardSelected2()
	{
		StopRewardSelection(2);
	}
}
