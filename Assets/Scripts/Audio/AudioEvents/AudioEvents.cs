using UnityEngine;
using UnityEngine.Audio;

public class AudioEvents
{
        
}

public struct SfxEvent
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RuntimeInitialization() { OnEvent = null; }
        
    public delegate void Delegate(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f, int priority = 128);

    static event Delegate OnEvent;

    public static void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    public static void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    public static void Trigger(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f, int priority = 128)
    {
        OnEvent?.Invoke(clipToPlay, audioGroup, volume, pitch, priority);
    }
}