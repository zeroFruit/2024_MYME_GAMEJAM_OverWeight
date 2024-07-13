using SSR.OverWeight;

public struct ElevatorRestingEvent
{
    public ElevatorController Elevator;
    private static ElevatorRestingEvent _e;

    public static void Trigger(ElevatorController elevator)
    {
        _e.Elevator = elevator;
        EventManager.TriggerEvent(_e);
    }
}

public struct ElevatorArrivalEvent
{
    public Floor ArrivalFloor;
    public int RemainWeight;
    public ElevatorDirection AfterDirection;
    public ElevatorController Elevator;
    private static ElevatorArrivalEvent _e;

    public static void Trigger(
        Floor arrivalFloor,
        int remainWeight,
        ElevatorDirection afterDirection,
        ElevatorController elevator
    )
    {
        _e.ArrivalFloor = arrivalFloor;
        _e.RemainWeight = remainWeight;
        _e.AfterDirection = afterDirection;
        _e.Elevator = elevator;
        EventManager.TriggerEvent(_e);
    }
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