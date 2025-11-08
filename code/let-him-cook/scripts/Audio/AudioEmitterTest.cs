using Godot;

public partial class AudioEmitterTest : Node
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
			AudioManager.Instance.PlaySound2D(SOUND_EFFECT_TYPE.Test);
		}
	}
}