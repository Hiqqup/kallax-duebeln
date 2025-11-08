using System;
using Godot;
using Godot.Collections;

public enum MUSIC_TYPE
{
	None,
	MainMenu,
	BackgroundMusic
}

public partial class AudioManager : Node2D
{
	public static AudioManager Instance { get; private set; }

	[Export] private AudioStreamPlayer musicPlayer;
		
	[Export] private Dictionary<SOUND_EFFECT_TYPE, SoundEffect> soundEffectDict; // Loads all registered SoundEffects on ready as a reference.
	[Export] private Array<SoundEffect> soundEffects; // Stores all possible SoundEffects that can be played
	
	public override void _Ready()
	{
		Instance = this;
		GD.Print("AudioManager Initialized.");
		foreach (var soundEffect in soundEffects)
		{
			soundEffectDict.Add(soundEffect.Type, soundEffect);
		}
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
		
		newSound.Bus = sfx.Bus;
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

	public void PlaySoundGlobal(SOUND_EFFECT_TYPE type)
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
		var newSound = new AudioStreamPlayer();
		AddChild(newSound);
		
		newSound.Bus = sfx.Bus;
		newSound.Stream = sfx.AudioStreamMp3;
		newSound.VolumeDb = sfx.Volume;
		newSound.PitchScale = sfx.PitchScale;
		RandomNumberGenerator rand = new RandomNumberGenerator();
		newSound.PitchScale += rand.RandfRange(-sfx.PitchRandomness, sfx.PitchRandomness);
		newSound.Finished += sfx.onAudioFinished;
		newSound.Finished += newSound.QueueFree;
		newSound.Play();
	}

	public void changeMusic(MUSIC_TYPE type)
	{
		AudioStreamPlaybackInteractive playback = musicPlayer.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		if (!IsInstanceValid(playback))
		{
			GD.PushError("cast to AudioStreamPlaybackInteractive returns invalid.");
			return;
		}
		
		var stream = musicPlayer.GetStream() as AudioStreamInteractive;
		if (!IsInstanceValid(stream))
		{
			GD.PushError("cast to AudioStreamInteractive returns invalid.");
			return;
		}
		
		var currentClipName = stream.GetClipName(playback.GetCurrentClipIndex());
		var nextClipName = type.ToString();
		//GD.Print("clip name: ", currentClipName, "/ type name: ", nextClipName); //Debug print
		if (currentClipName.Equals(nextClipName))
		{
			GD.PushWarning("Change to already playing clip is not allowed.");
			return;
		}
		
		playback.SwitchToClipByName(type.ToString());
	}

	public void toggleMusicPlayback()
	{
		musicPlayer.StreamPaused = !musicPlayer.StreamPaused;
	}
}
