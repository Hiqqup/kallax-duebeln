using System;
using Godot;
using Godot.Collections;
using LetHimCook.scripts.Audio;

public partial class AudioManager : Node2D
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
			PlaySound2D(SOUND_EFFECT_TYPE.Empty);
		}
	}

	/// <summary>
	/// Play sound effect on the given parent Node.
	/// </summary>
	/// <param name="parent">Node to play the sound effect on. </param>
	/// <param name="type">The Sound effect to be played.</param>
	public void PlaySound2D(Node2D parent, SOUND_EFFECT_TYPE type)
	{
		GD.Print("Playing 2d Sound effect");
		if (!soundEffectDict.ContainsKey(type))
		{
			GD.PushWarning("Audio Manager failed to find setting for type ", type);
			return;
		}

		SoundEffect sfx = soundEffectDict[type];
		if (!sfx.hasOpenLimit())
		{
			return;
		}

		sfx.changeAudioCount(1);
		var newSound = new AudioStreamPlayer2D();
		
		if (IsInstanceValid(parent))
		{
			parent.AddChild(newSound);
			newSound.Position = parent.Position;
		}
		else
		{
			GD.PushWarning("Parent is not valid for sound effect: ", type);
			this.AddChild(newSound);
			newSound.Position = this.Position;
		}
		
		newSound.Stream = sfx.AudioStreamMp3;
		newSound.VolumeDb = sfx.Volume;
		newSound.PitchScale = sfx.PitchScale;
		RandomNumberGenerator rand = new RandomNumberGenerator();
		newSound.PitchScale += rand.RandfRange(-sfx.PitchRandomness, sfx.PitchRandomness);
		newSound.Finished += sfx.onAudioFinished;
		newSound.Finished += newSound.QueueFree;
		newSound.Play();
		
	}

	public void PlaySound2D(SOUND_EFFECT_TYPE type)
	{
		PlaySound2D(this, type);
	}
}
