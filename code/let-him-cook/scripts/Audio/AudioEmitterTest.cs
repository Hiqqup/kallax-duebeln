using Godot;

public partial class AudioEmitterTest : Node2D
{
	[Export] public AudioStreamPlayer player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//test audio
		if (Input.IsKeyPressed(Key.K))
		{
			AudioManager.Instance.PlaySoundByType2D(this, SOUND_EFFECT.SharpPop);
		}
		//Preferred Method
		if (Input.IsKeyPressed(Key.I))
		{
			//player.Play();
			//AudioManager.Instance.PlaySound(player);
			
			//Preferred way
			AudioManager.Instance.PlayByPath("res://assets/audio/SoundEffects/boing.mp3");
		}
		
		
		// change music test
		if (Input.IsKeyPressed(Key.L))
		{
			AudioManager.Instance.changeMusic("BackgroundMusic");
		}
		if (Input.IsKeyPressed(Key.P))
		{
			AudioManager.Instance.toggleMusicPlayback();
		}
	}
}