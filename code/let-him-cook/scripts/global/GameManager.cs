using Godot;
using System;
using System.Linq;
using global::LetHimCook.scripts.global;

public partial class GameManager : Node
{
	[Export]
	private AnimationPlayer AnimationPlayer { get; set; }
	public Node CurrentWorld { get; private set; }
	public static GameManager Instance { get; private set; }
	
	public static int playerHealth = 100;
	public TechTierTracker CurrentTechTierTracker = new TechTierTracker();
	public int CurrenctScore = 0;
	[Export] public Reward reward { get; set; }

	[Export] public ProgressBar healthBar;
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
		healthBar.Value = playerHealth;
		if (Input.IsActionJustPressed("escape"))
		{
			modPlayerHealth(-100);
		}
	}

	public void LoadScene(string scenePath)
	{
		AnimationPlayer.Play("fade_to_black");
		ClearChildren();
		var scene = GD.Load<PackedScene>(scenePath);
		var sceneInstance = scene.Instantiate();
		CurrentWorld = sceneInstance;
		AddChild(sceneInstance);
		playerHealth = 100;
		AnimationPlayer.Play("fade_from_black");
	}
	
	public void LoadMainMenu()
	{
		healthBar.Hide();
		AudioManager.Instance.changeMusic("MainMenu");
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

	public static void setPlayerHealth(int health)
	{
		playerHealth = health;
	}

	public static void modPlayerHealth(int health)
	{
		playerHealth = int.Min(playerHealth + health, 100);
		if (playerHealth <= 0)
		{
			Instance.OnPlayerDeath();
		}
	}

	private void OnPlayerDeath()
	{
		if (CurrenctScore > LoadScore())
		{
			SaveScore(CurrenctScore);
		}
		CurrenctScore = 0;
		Instance.LoadMainMenu();
		playerHealth = 100;
	}

	public static int GetPlayerHealth()
	{
		return playerHealth;
	}
	
	public static void SaveScore(int score)
	{
		var config = new ConfigFile();
		config.SetValue("SaveData", "HighScore", score);

		var err = config.Save("user://save_data.cfg");

		if (err != Error.Ok)
		{
			GD.PrintErr("Failed to save score!");
		}
	}
    
	public static int LoadScore()
	{
		var config = new ConfigFile();
		var err = config.Load("user://save_data.cfg");

		if (err != Error.Ok)
			return 0; // Default score if file doesn't exist

		return (int)config.GetValue("SaveData", "HighScore", 0);
	}
}
