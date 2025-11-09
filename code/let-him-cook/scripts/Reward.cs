using Godot;
using System;
using Godot.Collections;

public partial class Reward : Control
{
	private Action<int> _callback;
	private Array<I_GraphNode> _rewards = new Array<I_GraphNode>();
	private Array<Button> _rewardButtons = new Array<Button>();
	// Called when the node enters the scene tree for the first time.
	
	private void GenerateRewards()
	{
		_rewards.Clear();
		for (int i = 0; i < Constants.ChoiceCount; i++)
		{
			var inputs = new Array<ResourceAmount>();
			var outputs = new Array<ResourceAmount>();
			if (i == 0)
			{
				// slot 0 is always a producer of a random T1 resource
				ProductionResource randomResource = ProductionResourceExtensions.GetRandomT1Resource();
				outputs.Add(new ResourceAmount(randomResource, GD.RandRange(1,5)));
			}
			else
			{
				var inputMaterialsTier = GD.RandRange(1, GameManager.Instance.CurrentTechTierTracker.MaxUnlockedTier ); 
				
				var amountOfInputs = GD.RandRange(1, 3);
				Dictionary<ProductionResource, int> cost = new Dictionary<ProductionResource, int>();
				for (int j = 0; j < amountOfInputs; j++)
				{
					ProductionResource randomResource = ProductionResourceExtensions.GetRandomTierResource(inputMaterialsTier);
					var amountToAdd = GD.RandRange(1,5);
					if (cost.ContainsKey(randomResource))
					{
						cost[randomResource] += amountToAdd;
					}
					else
					{
						cost.Add(randomResource, amountToAdd);
					}
				}

				int inputAmounts = 0;
				foreach (var kvp in cost)
				{
					inputs.Add(new ResourceAmount(kvp.Key, kvp.Value));
					inputAmounts+= kvp.Value;
				}
				outputs.Add(new ResourceAmount(ProductionResourceExtensions.GetRandomTierResource(inputMaterialsTier+1), inputAmounts));
			}
			
			_rewards.Add(new I_GraphNode(inputs, outputs));
		}
	}

	private void UpdateLabels()
	{
		for (int i = 0; i < Constants.ChoiceCount; i++)
		{
			string label = "";

			if (_rewards[i].Recource_Input.Count > 0)
			{
				label += "IN: \n";
				foreach (var input in _rewards[i].Recource_Input)
				{
					label += ( input.Resource.ToString() + " x" + input.Amount.ToString() ) + "\n";
				}
			}

			if (_rewards[i].Output.Count > 0)
			{
				label += "OUT: \n";
				label += (_rewards[i].Output[0].Resource.ToString() + " x" + _rewards[i].Output[0].Amount.ToString());
				label += "\n";
			}
			
			_rewardButtons[i].Text = label;
		}
	}
	
	public override void _Ready()
	{
		_rewardButtons.Add(GetNode<Button>("Button"));
		_rewardButtons.Add(GetNode<Button>("Button2"));
		_rewardButtons.Add(GetNode<Button>("Button3"));
		
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
		GenerateRewards();
		UpdateLabels();
		 Visible = true;
		_callback = c;
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/turn-page.mp3");
	}

	private void CallbackWasntSetWarning(int i)
	{
		GD.Print($"WARNING: CALLBACK WASN'T SET");
	}
	
	private void StopRewardSelection(int i)
	{
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/keyboard-click.mp3");
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
