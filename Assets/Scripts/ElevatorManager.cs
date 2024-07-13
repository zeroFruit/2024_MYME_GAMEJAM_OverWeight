using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorManager : Singleton<ElevatorManager>, EventListener<ElevatorRestingEvent>
{
    private List<ElevatorController> _elevators = new List<ElevatorController>();

    public void Init()
    {
        // test용
        ElevatorController elevatorController = GameObject.Find("elevator").GetComponent<ElevatorController>();
        _elevators.Add(elevatorController);
        // todo : elevator 추가
    }

    public void Update()
    {
        // 1. 큐잉 끊어줘야할 게 있는지 체크 후 끊기
        // 2. 빈 큐 사람들 맞는거 있는지 호출하기
        // 3. 엘레베이터 타겟 플로어 수정해주기

        // 1. 큐잉 끊어줘야할 게 있는지 체크 후 끊기
        List<Passenger> queuedPassengers = PassengerManager.Instance.GetQueuedPassengers();
        foreach (var queuedPassenger in queuedPassengers)
        {
            if (!queuedPassenger.queuedElevator.CanEnterCapacity(queuedPassenger))
            {
                queuedPassenger.queuedElevator = null;
            }
        }

        // 2. 빈 큐 사람들 맞는거 있는지 호출하기
        List<Passenger> unQueuedPassengers = PassengerManager.Instance.GetUnQueuedPassengers();
        foreach (var unQueuedPassenger in unQueuedPassengers)
        {
            ElevatorController selectedElevator = null;
            // 2.1 가는방향에 움직이고 있는 엘레베이터 있으면 인터셉트
            foreach (var ev in _elevators)
            {
                if (ev.CurrentState != ElevatorController.ElevatorState.MOVING)
                    continue;
                if (ev.CanStop(unQueuedPassenger.StartFloor) && !ev.IsFull())
                {
                    selectedElevator = ev;
                    break;
                }
            }

            if (selectedElevator != null)
            {
                unQueuedPassenger.queuedElevator = selectedElevator;
                continue;
            }

            // 2.2 가는방향에 움직이는 엘레베이터가 없고 IDLE 이 있으면 인터셉트
            foreach (var ev in _elevators)
            {
                if (ev.CurrentState != ElevatorController.ElevatorState.IDLE)
                    continue;
                if (!ev.IsFull())
                {
                    selectedElevator = ev;
                    break;
                }
            }

            if (selectedElevator != null)
            {
                unQueuedPassenger.queuedElevator = selectedElevator;
                continue;
            }

            // 2.3 그마저도없으면 ON/OFF BOARDING 방향같은 엘레베이터에 QUEUE
            foreach (var ev in _elevators)
            {
                if (ev.CanStop(unQueuedPassenger.StartFloor) && !ev.IsFull())
                {
                    selectedElevator = ev;
                    break;
                }
            }

            if (selectedElevator != null)
            {
                unQueuedPassenger.queuedElevator = selectedElevator;
                continue;
            }

            // 암것도안되면 queue가안됌
        }

        // 3. 엘레베이터 타겟 플로어 수정해주기
        foreach (var ev in _elevators)
        {
            RouteElevator(ev);
        }
    }

    public void RouteElevator(ElevatorController ev)
    {
        List<Passenger> queuedPassengers = PassengerManager.Instance.GetQueuedPassengers(ev);
        List<Passenger> inElevatorPassengers = ev.Passengers.ToList();
        queuedPassengers.Sort((p1, p2) =>
        {
            float yDiff1 = ev.transform.position.y - p1.StartFloor.transform.position.y;
            float yDiff2 = ev.transform.position.y - p2.StartFloor.transform.position.y;
            if (ev.MoveStartFloor.DirectionTo(ev.TargetFloor) == ElevatorDirection.Down)
            {
                yDiff1 = -yDiff1;
                yDiff2 = -yDiff2;
            }

            return yDiff1.CompareTo(yDiff2);
        });

        inElevatorPassengers.Sort((p1, p2) =>
        {
            if (ev.MoveStartFloor.DirectionTo(ev.TargetFloor) == ElevatorDirection.Down)
            {
                return p2.TargetFloor.FloorIdx.CompareTo(p1.TargetFloor.FloorIdx);
            }
            else
            {
                return p1.TargetFloor.FloorIdx.CompareTo(p2.TargetFloor.FloorIdx);
            }
        });


        Passenger nearestQueuedPassenger = queuedPassengers.FirstOrDefault();
        Passenger nearestInElevatorPassenger = inElevatorPassengers.FirstOrDefault();

        if (nearestQueuedPassenger != null && nearestInElevatorPassenger != null)
        {
            if (Math.Abs(ev.DistanceFromHere(nearestQueuedPassenger.StartFloor)) >
                Math.Abs(ev.DistanceFromHere(nearestInElevatorPassenger.TargetFloor)) &&
                ev.CanStop(nearestInElevatorPassenger.TargetFloor))
            {
                ev.TargetFloor = nearestInElevatorPassenger.TargetFloor;
            }
            else if (Math.Abs(ev.DistanceFromHere(nearestQueuedPassenger.StartFloor)) <
                     Math.Abs(ev.DistanceFromHere(nearestInElevatorPassenger.TargetFloor)) &&
                     ev.CanStop(nearestQueuedPassenger.StartFloor))
            {
                ev.TargetFloor = nearestInElevatorPassenger.TargetFloor;
            }
        }

        if (nearestQueuedPassenger != null && nearestInElevatorPassenger == null)
        {
            if (ev.CanStop(nearestQueuedPassenger.StartFloor))
            {
                ev.TargetFloor = nearestQueuedPassenger.StartFloor;
            }
        }

        if (nearestQueuedPassenger == null && nearestInElevatorPassenger != null)
        {
            if (ev.CanStop(nearestInElevatorPassenger.TargetFloor))
            {
                ev.TargetFloor = nearestInElevatorPassenger.TargetFloor;
            }
        }
    }

    public bool Enter(Passenger passenger)
    {
        ElevatorDirection dir = passenger.StartFloor.DirectionTo(passenger.TargetFloor);
        ElevatorController elevator = GetCurrentFloorElevator(passenger.StartFloor, dir);
        if (elevator == null)
        {
            Debug.Log("cannot find available elevator!");
            return false;
        }

        return elevator.Enter(passenger);
    }

    private ElevatorController GetCurrentFloorElevator(Floor floor, ElevatorDirection dir)
    {
        foreach (var elevator in _elevators)
        {
            if (
                elevator.MoveStartFloor == floor &&
                elevator.CurrentState == ElevatorController.ElevatorState.ONBOARDING &&
                (elevator.AfterDirection() == ElevatorDirection.UNWARE || elevator.AfterDirection() == dir)
            )
                return elevator;
        }

        return null;
    }

    public void OnEvent(ElevatorRestingEvent e)
    {
        // 쉬고있으니 필요하면 써야함
        List<Passenger> waitingPassengers = PassengerManager.Instance.GetAllPassengers();
        if (waitingPassengers.Count == 0)
            return;
        waitingPassengers.Sort((p1, p2) =>
        {
            int diff1 = Math.Abs(p1.StartFloor.FloorIdx - e.Elevator.MoveStartFloor.FloorIdx);
            int diff2 = Math.Abs(p2.StartFloor.FloorIdx - e.Elevator.MoveStartFloor.FloorIdx);
            return diff1.CompareTo(diff2);
        });
        Passenger nearestPassenger = waitingPassengers.First();
        e.Elevator.TargetFloor = nearestPassenger.StartFloor;
        Debug.Log($"Route To waiting elevator passenger. to : ${e.Elevator.TargetFloor}");
    }


    void OnEnable()
    {
        this.StartListeningEvent();
    }

    void OnDisable()
    {
        this.StopListeningEvent();
    }

    public int SpawnFloorIdx = 0;
    [InspectorButton("SpawnButton")] public bool TestButton;

    void SpawnButton()
    {
        Debug.Log("HEREEREERRE");
    }
}