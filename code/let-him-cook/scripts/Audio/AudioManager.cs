using Godot;
using Godot.Collections;
using LetHimCook.scripts.Audio;

public partial class AudioSystem : Node
{
	private Dictionary<SOUND_EFFECT_TYPE, SoundEffect> soundEffectDict; // Loads all registered SoundEffects on ready as a reference.

	[Export] private Array<SoundEffect> soundEffects; // Stores all possible SoundEffects that can be played
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var soundEffect in soundEffects)
		{
			soundEffectDict[soundEffect.Type] = soundEffect;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.K))
		{
			//test audio
			PlaySound();
		}
	}

	public void PlaySound(Vector2 position)
	{
		var newSound = new AudioStreamPlayer2D();
		AddChild(newSound);
		newSound.Position = position;
		//newSound.Stream = ...;
		
		newSound.Play();
		
	}

	public void PlaySound()
	{
		PlaySound(new Vector2(0, 0));
	}
}
