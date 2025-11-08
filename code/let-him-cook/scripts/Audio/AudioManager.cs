using System;
using Godot;
using Godot.Collections;

public partial class AudioManager : Node2D
{
	 public static AudioManager Instance { get; private set; }
		
	[Export] private Dictionary<SOUND_EFFECT_TYPE, SoundEffect> soundEffectDict; // Loads all registered SoundEffects on ready as a reference.

	[Export] private Array<SoundEffect> soundEffects; // Stores all possible SoundEffects that can be played
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		GD.Print("AudioManager Initialized.");
		foreach (var soundEffect in soundEffects)
		{
			soundEffectDict.Add(soundEffect.Type, soundEffect);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	/// <summary>
	/// Play sound effect on the given parent Node.
	/// </summary>
	/// <param name="parent">Node to play the sound effect on. </param>
	/// <param name="type">The Sound effect to be played.</param>
	public void PlaySound2D(Node2D parent, SOUND_EFFECT_TYPE type)
	{
		if (!soundEffectDict.ContainsKey(type))
		{
			GD.PushWarning("Audio Manager failed to find setting for type ", type);
			return;
		}

		SoundEffect sfx = soundEffectDict[type];
		if (!sfx.hasOpenLimit())
		{
			//GD.PushWarning("Sound has reached limit, skipping.");
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
