using Godot;
using System;

public partial class Reward : Node
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
		_callback = c;
	}

	private void CallbackWasntSetWarning(int i)
	{
		GD.Print($"WARNING: CALLBACK WASN'T SET");
	}

	public void RewardSelected0()
	{
		_callback(0);
		GD.Print("0");
	}
	public void RewardSelected1()
	{
		_callback(1);
		GD.Print("1");
	}
	public void RewardSelected2()
	{
		_callback(2);
		GD.Print("2");
	}
}
