using SSR.OverWeight;

public enum UpgradeEventType
{
    Updated,
}

public struct UpgradeEvent
{
    public UpgradeEventType EventType;

    static UpgradeEvent e;

    public static void Trigger(UpgradeEventType eventType)
    {
        e.EventType = eventType;
        EventManager.TriggerEvent(e);
    }
}