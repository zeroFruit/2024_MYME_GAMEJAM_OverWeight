using SSR.OverWeight;
using UnityEngine;

public enum SoundManagerSoundControlEventTypes
{
    Pause,
    Resume,
    Stop,
    Free,
}
    
/// <summary>
/// An event used to control a specific sound on the MMSoundManager.
/// You can either search for it by ID, or directly pass an audiosource if you have it.
/// </summary>
public struct SoundManagerSoundControlEvent
{
    /// the ID of the sound to control (has to match the one used to play it)
    public int SoundID;
    /// the control mode
    public SoundManagerSoundControlEventTypes EventType;
    /// the audiosource to control (if specified)
    public AudioSource TargetSource;

    static SoundManagerSoundControlEvent e;

    public static void Trigger(SoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
    {
        e.SoundID = soundID;
        e.TargetSource = source;
        e.EventType = eventType;
        EventManager.TriggerEvent(e);
    }
}