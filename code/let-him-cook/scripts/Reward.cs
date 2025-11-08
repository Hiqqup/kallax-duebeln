using Godot;
using System;

public partial class Reward : Control
{
	private Action<int> _callback;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_callback = CallbackWasntSetWarning;
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
