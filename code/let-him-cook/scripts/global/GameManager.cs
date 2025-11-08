using Godot;
using System;
using System.Linq;
using global::LetHimCook.scripts.global;

public partial class GameManager : Node
{
	[Export]
	private AnimationPlayer AnimationPlayer { get; set; }
	public static GameManager Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		base._Ready();
		LoadMainMenu();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadScene(string scenePath)
	{
		AnimationPlayer.Play("fade_to_black");
		ClearChildren();
		var scene = GD.Load<PackedScene>(scenePath);
		var sceneInstance = scene.Instantiate();
		AddChild(sceneInstance);
		AnimationPlayer.Play("fade_from_black");
	}
	
	public void LoadMainMenu()
	{
		LoadScene(PathLookup.MainMenuPath);
	}

	private void ClearChildren()
	{
		var children = GetChildren().Where(x => x.Name != "Internal").ToList();
		if (children.Count > 0)
		{
			foreach (var child in children)
			{
				RemoveChild(child);
				child.QueueFree();
			}
		}
	}

	public void CloseGame()
	{
		GetTree().Quit();
	}
}
