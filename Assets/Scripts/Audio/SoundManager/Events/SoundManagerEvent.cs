using SSR.OverWeight;

public enum SoundManagerEventTypes
{
    SaveSettings,
    LoadSettings,
    SettingsLoaded,
}
    
/// <summary>
/// This event will let you trigger a save/load/reset on the MMSoundManager settings
/// </summary>
public struct SoundManagerEvent
{
    public SoundManagerEventTypes EventType;
        
    static SoundManagerEvent e;

    public static void Trigger(SoundManagerEventTypes eventType)
    {
        e.EventType = eventType;
        EventManager.TriggerEvent(e);
    }
}