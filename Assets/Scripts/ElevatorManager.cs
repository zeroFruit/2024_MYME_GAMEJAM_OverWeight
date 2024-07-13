using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine.EventSystems;

public class ElevatorManager : Singleton<ElevatorManager>, EventListener<ElevatorRestingEvent>
{
    public static bool Enter(List<Passenger> passengers)
    {
        throw new System.NotImplementedException();
    }
    private List<ElevatorController> _elevators;

    public void Init()
    {
        // todo : elevator 추가
    }

    public bool Enter(Passenger passenger)
    {
        ElevatorDirection dir = passenger.TargetFloor - passenger.StartFloor;
        ElevatorController elevator = GetAvailableElevator(passenger.StartFloor, dir);
        if (elevator == null)
        {
            return false;
        }

        return elevator.Enter(passenger);
    }

    private ElevatorController GetAvailableElevator(Floor floor, ElevatorDirection dir)
    {
        foreach (var elevator in _elevators)
        {
            if (
                elevator.CurrentFloor == floor &&
                elevator.CurrentState == ElevatorController.ElevatorState.ONBOARDING &&
                elevator.AfterDirection() == dir
            )
                return elevator;
        }

        return null;
    }

    public void OnEvent(ElevatorRestingEvent e)
    {
        // 쉬고있으니 필요하면 써야함
    }

    public void routeElevator(ElevatorController elevatorController, ElevatorDirection moveDirection)
    {
        List<Passenger> waitingPassengers = PassengerManager.Instance.GetMinimalSizePassengerPerFloor();
        List<Passenger> inElevatorPassengers = elevatorController.Passengers;
        // todo
        List<Passenger> candidatePassengers = new List<Passenger>();
        foreach (var waitingPassenger in waitingPassengers)
        {
            if (moveDirection == ElevatorDirection.UNWARE)
            {
                candidatePassengers.Add(waitingPassenger);
            }

            ElevatorDirection waitingPassengerDirection = waitingPassenger.TargetFloor - waitingPassenger.StartFloor;
            // if (moveDirection == ElevatorDirection.Up && 
                // elevatorController.CurrentFloor == ElevatorDirection.Up)
            // {
            // }
        }

        elevatorController.TargetFloor = null;
        throw new System.NotImplementedException();
    }

    void OnEnable()
    {
        this.StartListeningEvent();
    }

    void OnDisable()
    {
        this.StopListeningEvent();
    }
}