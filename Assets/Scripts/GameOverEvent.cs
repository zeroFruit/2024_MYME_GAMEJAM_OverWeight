using SSR.OverWeight;

public struct GameOverEvent
{
    static GameOverEvent e;
    
    public static void Trigger()
    {
        EventManager.TriggerEvent(e);
    }
}