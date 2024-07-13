using UnityEngine;

/// <summary>
/// This event will let you play a sound on the SoundManager
///
/// Example : SoundManagerSoundPlayEvent.Trigger(ExplosionSfx, SoundManager.SoundManagerTracks.Sfx, this.transform.position);
/// will play a clip (here ours is called ExplosionSfx) on the SFX track, at the position of the object calling it
/// </summary>
public struct SoundManagerSoundPlayEvent
{
    public delegate AudioSource Delegate(AudioClip clip, SoundManagerPlayOptions options);

    static event Delegate OnEvent;
        
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RuntimeInitialization() { OnEvent = null; }

    public static void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    public static void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    public static AudioSource Trigger(AudioClip clip, SoundManagerPlayOptions options)
    {
        return OnEvent?.Invoke(clip, options);
    }
}