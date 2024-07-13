using SSR.OverWeight;

public enum DayEventType
{
    /// <summary>
    /// 하루 시간 업데이트될 때 이벤트
    /// </summary>
    ProgressUpdated,
    
    DayStarted,
    
    DayEnded,
    
    WaveStarted,
    
    WaveEnded,
}

public struct DayEvent
{
    public DayEventType EventType;
    public object Args;

    static DayEvent e;

    public static void Trigger(DayEventType eventType)
    {
        e.EventType = eventType;
        EventManager.TriggerEvent(e);
    }

    public static void Trigger(DayEventType eventType, object args)
    {
        e.EventType = eventType;
        e.Args = args;
        EventManager.TriggerEvent(e);
    }
}