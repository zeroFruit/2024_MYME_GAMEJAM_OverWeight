using SSR.OverWeight;

public struct GoldChangedEvent
{
    public int Plus;
    public int Minus;
    public int From;
    public int To;
    private static GoldChangedEvent _e;

    public static void Trigger(
        int plus,
        int minus,
        int from,
        int to
    )
    {
        _e.Plus = plus;
        _e.Minus = minus;
        _e.From = from;
        _e.To = to;
        EventManager.TriggerEvent(_e);
    }
}