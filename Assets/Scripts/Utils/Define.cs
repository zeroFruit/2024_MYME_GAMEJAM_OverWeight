public enum ElevatorDirection
{
    Up = 1,
    Down = -1,
    UNWARE = 0
}

public static class ElevatorDirectionExtensions
{
    public static float ToFloat(this ElevatorDirection direction)
    {
        switch (direction)
        {
            case ElevatorDirection.Up:
                return 1f;
            case ElevatorDirection.Down:
                return -1f;
            default:
                return 0f;
        }
    }
}