using Godot;

public partial class AudioEmitterTest : Node2D
{
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
			AudioManager.Instance.PlaySound2D(this, SOUND_EFFECT_TYPE.Test);
		}
		// change music test
		if (Input.IsKeyPressed(Key.L))
		{
			AudioManager.Instance.changeMusic(MUSIC_TYPE.BackgroundMusic);
		}
		
		if (Input.IsKeyPressed(Key.P))
		{
			AudioManager.Instance.toggleMusicPlayback();
		}
	}
}