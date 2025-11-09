using Godot;
using System;
using Godot.Collections;
using Godot.NativeInterop;

public partial class MainMenu : Control
{
	
	[Export] public Label CreditsLabel;
	[Export] public Label TutorialLabel;
	[Export] public Button StartButton;
	[Export] public Button ExitButton;
	[Export] public Button CreditsButton;
	[Export] public VideoStreamPlayer KallaxVideo;
	[Export] public Label ScoreLabel;
	private Godot.GDScript _animationUtil;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationUtil= GD.Load<Godot.GDScript>("res://scripts/dont_look/animation_util.gd");
		Array<Button> buttons = [StartButton, ExitButton, CreditsButton];
		foreach (Button button in buttons)
		{
			button.PivotOffset = button.Size/2;
			button.Connect("mouse_entered", Callable.From(() => 
				// add in soud effect here
				_animationUtil.Call("bounce_tween", button)
				));
		}
		if(GameManager.LoadScore() > 0)
		{
			ScoreLabel.Show();
			ScoreLabel.Text = $"Highscore: {GameManager.LoadScore()}";
		}
		else
		{
			ScoreLabel.Hide();
		}
	}

	private void StartGame()
	{
		
		GameManager.Instance.healthBar.Visible = true;
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/sharp-pop.mp3");
		AudioManager.Instance.changeMusic("BackgroundMusic");
		GameManager.Instance.LoadScene("res://scenes/test.tscn");
	}

	private void ExitGame()
	{
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/sharp-pop.mp3");
		GameManager.Instance.CloseGame();
	}


	private void ToggleCredits()
	{
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/sharp-pop.mp3");
		if (CreditsLabel.Visible)
		{
			KallaxVideo.Visible = true;
			CreditsLabel.Visible = false;
			TutorialLabel.Visible = false;
		}
		else
		{
			KallaxVideo.Visible = false;
			CreditsLabel.Visible = true;
			TutorialLabel.Visible = false;
		}
		
	}

	private void ToggleTutorial()
	{
		AudioManager.Instance.PlayAudio("res://assets/audio/SoundEffects/sharp-pop.mp3");
		if (TutorialLabel.Visible)
		{
			KallaxVideo.Visible = true;
			CreditsLabel.Visible = false;
			TutorialLabel.Visible = false;
		}
		else
		{
			KallaxVideo.Visible = false;
			CreditsLabel.Visible = false;
			TutorialLabel.Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
