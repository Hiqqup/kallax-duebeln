using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void StartGame()
	{
		GameManager.Instance.LoadScene("res://scenes/test.tscn");
	}

	private void ExitGame()
	{
		GameManager.Instance.CloseGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
