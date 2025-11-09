using System;
using System.Collections.Generic;
using Godot;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance { get; private set; }

	[Export] private AudioStreamPlayer _musicPlayer;
	private Dictionary<string, AudioStreamMP3> _audioStreamsDict = new Dictionary<string, AudioStreamMP3>();
	private const string SfxPath = "res://assets/audio/SoundEffects/";
	
	public override void _Ready()
	{
		Instance = this;
		LoadAudioStreamsFromPath();
	}
	

	private void LoadAudioStreamsFromPath()
	{
		var dir = DirAccess.Open(SfxPath);
		if (dir == null)
		{
			GD.PrintErr("Could not open folder");
			return;
		}
		dir.ListDirBegin();
		foreach (var file in dir.GetFiles())
		{
			if (file.Contains(".import"))
			{
				continue;
			}
			var resPath = SfxPath + file;
			//GD.Print("path: ", resPath);
			var sound = ResourceLoader.Load(resPath) as AudioStreamMP3;
			if (!IsInstanceValid(sound))
			{
				GD.PrintErr("Sound resource could not be loaded.");
			}
			_audioStreamsDict.Add(resPath, sound);
		}
		dir.ListDirEnd();
	}


	/// <summary>
	/// Changes the currently playing audio track to the next clip
	/// </summary>
	/// <param name="clipName">The exact string name of the clip that should be played. Needs to be exactly as in the attached AudioStreamPlayer</param>
	public void changeMusic(string clipName)
	{
		AudioStreamPlaybackInteractive playback = _musicPlayer.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		if (!IsInstanceValid(playback))
		{
			GD.PrintErr("cast to AudioStreamPlaybackInteractive returns invalid.");
			return;
		}
		
		var stream = _musicPlayer.GetStream() as AudioStreamInteractive;
		if (!IsInstanceValid(stream))
		{
			GD.PrintErr("cast to AudioStreamInteractive returns invalid.");
			return;
		}
		var currentClipName = stream.GetClipName(playback.GetCurrentClipIndex());
		//GD.Print("clip name: ", currentClipName, "/ type name: ", nextClipName); //Debug print
		if (currentClipName.Equals(clipName))
		{
			GD.Print("WARNING: Tried changing to an already playing clip.");
			return;
		}
		
		playback.SwitchToClipByName(clipName);
	}

	public void toggleMusicPlayback()
	{
		_musicPlayer.StreamPaused = !_musicPlayer.StreamPaused;
	}

	
	
	
	public void PlayAudio(string resPath)
	{
		var stream = _audioStreamsDict[resPath];
		var player = new AudioStreamPlayer();
		this.AddChild(player);
		player.Stream = stream;
		//TODO limit logic here
		player.Play();
	}
	
	public void PlayAudio2D(string resPath, Node2D parent)
	{
		var stream = _audioStreamsDict[resPath];
		var player = new AudioStreamPlayer2D();
		parent.AddChild(player);
		player.Position = parent.Position;
		player.Stream = stream;
		//TODO limit logic here
		player.Play();
	}
	


	
	
	
	/*		OLD AND DEPRICATED
	/// <summary>
	/// Play sound effect on the given parent Node.
	/// </summary>
	/// <param name="parent">Node to play the sound effect on. </param>
	/// <param name="type">The Sound effect to be played.</param>
	public void PlaySoundByType2D(Node2D parent, SOUND_EFFECT type)
	{
		if (!_soundEffectDict.ContainsKey(type))
		{
			GD.PushWarning("Audio Manager failed to find setting for type ", type);
			return;
		}

		SoundEffect sfx = _soundEffectDict[type];
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
	

	public void PlaySoundByType(SOUND_EFFECT type)
	{
		if (!_soundEffectDict.ContainsKey(type))
		{
			GD.PushWarning("Audio Manager failed to find setting for type ", type);
			return;
		}

		SoundEffect sfx = _soundEffectDict[type];
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
	} */
}
