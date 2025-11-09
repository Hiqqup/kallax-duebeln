using Godot;

public enum SOUND_EFFECT {
    Empty,
    SharpPop
}
public partial class SoundEffect : Resource
{
    [Export] public string Name = "";
    [Export(PropertyHint.Range, "0,20,")] public int Limit { get; set; } = 0;
    [Export] public SOUND_EFFECT Type;
    [Export] public AudioStreamMP3 AudioStreamMp3;
    [Export(PropertyHint.Range, "-40,20,")] public float Volume { get; set; } = 0;
    [Export(PropertyHint.Range, "0.0, 4.0, .01")] public float PitchScale { get; set; } = 1.0f;
    [Export(PropertyHint.Range, "0.0, 1.0, .01")] public float PitchRandomness { get; set; } = 0.0f;
    [Export] public StringName Bus = "SFX";
    
    private int playingCount = 0;
    
    
    //Takes [param amount] to change the [member playingCount]. 
    public void changeAudioCount(int amount)
    {
        playingCount = int.Max(0, playingCount + amount);
    }

    //Checkes whether the audio limit is reached.
    public bool hasOpenLimit()
    {
        if (Limit == 0) return true;
        return playingCount < Limit;
    }

    //Connected to the [member AudioStreamMp3]'s finished signal to decrement the [member playingCount].
    public void onAudioFinished()
    {
        changeAudioCount(-1);
    }
}