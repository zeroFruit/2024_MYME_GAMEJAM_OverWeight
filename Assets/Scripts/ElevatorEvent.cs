using SSR.OverWeight;

public struct ElevatorArrivalEvent
{
    public Floor ArrivalFloor;
    public Floor StartFloor;
    public int RemainWeight;
    public ElevatorDirection AfterDirection;
}

public struct ElevatorPassengerEnteredEvent
{
    public Passenger Passenger;
    private static ElevatorPassengerEnteredEvent _e;

    public static void Trigger(Passenger passenger)
    {
        _e.Passenger = passenger;
        EventManager.TriggerEvent(_e);
    }
}

public struct ElevatorPassengerExitEvent
{
    public Passenger Passenger;
    private static ElevatorPassengerExitEvent _e;

    public static void Trigger(Passenger passenger)
    {
        _e.Passenger = passenger;
        EventManager.TriggerEvent(_e);
    }
}