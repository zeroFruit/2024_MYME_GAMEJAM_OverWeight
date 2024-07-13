using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorManager : Singleton<ElevatorManager>, EventListener<ElevatorRestingEvent>
{
    public List<ElevatorController> _elevators = new List<ElevatorController>();
    public GameObject ElevatorPrefab;

    public void Init()
    {
        float floorY = FloorManager.Instance.Floors.First().transform.position.y;
        GameObject ev1 = Instantiate(ElevatorPrefab);
        ev1.transform.position = new Vector2(11, floorY);
        ElevatorController elevator1 = ev1.GetComponent<ElevatorController>();
        elevator1.Init(0,
            FloorManager.Instance.Floors,
            FloorManager.Instance.Floors.First(), 4, 1);
        _elevators.Add(elevator1);
        UpgradeManager.Instance.AddElevator();
    }


    public void AddElevator(int buyIdx)
    {
        float floorY = FloorManager.Instance.Floors.First().transform.position.y;
        GameObject ev2 = Instantiate(ElevatorPrefab);
        ev2.transform.position = new Vector2(11 + 5.5f * buyIdx, floorY);
        ElevatorController elevator2 = ev2.GetComponent<ElevatorController>();
        elevator2.Init(buyIdx,
            FloorManager.Instance.Floors,
            FloorManager.Instance.Floors.First(), 4, 1);
        _elevators.Add(elevator2);
        UpgradeManager.Instance.AddElevator();
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
                continue;
            }

            if (queuedPassenger.queuedElevator.CurrentDirection != ElevatorDirection.UNWARE &&
                queuedPassenger.queuedElevator.CurrentDirection !=
                queuedPassenger.StartFloor.DirectionTo(queuedPassenger.TargetFloor))
            {
                queuedPassenger.queuedElevator = null;
                continue;
            }

            if (queuedPassenger.queuedElevator.CurrentDirection == ElevatorDirection.Up &&
                queuedPassenger.queuedElevator.DistanceFromHere(queuedPassenger.StartFloor) > 0)
            {
                queuedPassenger.queuedElevator = null;
                continue;
            }

            if (queuedPassenger.queuedElevator.CurrentDirection == ElevatorDirection.Down &&
                queuedPassenger.queuedElevator.DistanceFromHere(queuedPassenger.StartFloor) < 0)
            {
                queuedPassenger.queuedElevator = null;
                continue;
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
                if (ev.CanStop(unQueuedPassenger.StartFloor) && !ev.IsFull() &&
                    ev.IsStoppable(unQueuedPassenger.StartFloor, unQueuedPassenger.TargetFloor))
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
                if (!ev.IsFull() && ev.CurrentDirection == ElevatorDirection.UNWARE &&
                    ev.IsStoppable(unQueuedPassenger.StartFloor, unQueuedPassenger.TargetFloor))
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
                ElevatorDirection unQueuedDirection =
                    unQueuedPassenger.StartFloor.DirectionTo(unQueuedPassenger.TargetFloor);
                if (ev.CurrentDirection == unQueuedDirection && ev.CanStop(unQueuedPassenger.StartFloor) &&
                    ev.IsStoppable(unQueuedPassenger.StartFloor, unQueuedPassenger.TargetFloor))
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
            if (ev.CurrentDirection == ElevatorDirection.Up)
            {
                float yDiff1 = p1.StartFloor.transform.position.y - ev.transform.position.y;
                float yDiff2 = p2.StartFloor.transform.position.y - ev.transform.position.y;
                if (yDiff1 < 0) yDiff1 = 999999; // todo : something big
                if (yDiff2 < 0) yDiff2 = 999999; // todo : something big
                return yDiff1.CompareTo(yDiff2);
            }
            else if (ev.CurrentDirection == ElevatorDirection.Down)
            {
                float yDiff1 = ev.transform.position.y - p1.StartFloor.transform.position.y;
                float yDiff2 = ev.transform.position.y - p2.StartFloor.transform.position.y;
                if (yDiff1 < 0) yDiff1 = 999999; // todo : something big
                if (yDiff2 < 0) yDiff2 = 999999; // todo : something big
                return yDiff1.CompareTo(yDiff2);
            }
            else
            {
                float yDiff1 = Math.Abs(p1.StartFloor.transform.position.y - ev.transform.position.y);
                float yDiff2 = Math.Abs(p2.StartFloor.transform.position.y - ev.transform.position.y);
                return yDiff1.CompareTo(yDiff2);
            }
        });

        inElevatorPassengers.Sort((p1, p2) =>
        {
            if (ev.CurrentDirection == ElevatorDirection.Up)
            {
                return p1.TargetFloor.FloorIdx.CompareTo(p2.TargetFloor.FloorIdx);
            }
            else if (ev.CurrentDirection == ElevatorDirection.Down)
            {
                return p2.TargetFloor.FloorIdx.CompareTo(p1.TargetFloor.FloorIdx);
            }
            else
            {
                Debug.Log("어라 여기오면 안되는데..  in elevator sort");
                return p1.TargetFloor.FloorIdx.CompareTo(p2.TargetFloor.FloorIdx);
            }
        });


        Passenger nearestQueuedPassenger =
            queuedPassengers.FirstOrDefault();
        Passenger nearestInElevatorPassenger =
            inElevatorPassengers.FirstOrDefault();

        if (nearestQueuedPassenger != null && nearestInElevatorPassenger != null)
        {
            if (Math.Abs(ev.DistanceFromHere(nearestQueuedPassenger.StartFloor)) >
                Math.Abs(ev.DistanceFromHere(nearestInElevatorPassenger.TargetFloor)))
            {
                if (ev.TargetFloor != nearestInElevatorPassenger.TargetFloor)
                {
                    Debug.Log(
                        $"Target changed from1 {ev.TargetFloor.FloorIdx} -> {nearestInElevatorPassenger.TargetFloor}");
                    ev.TargetFloor = nearestInElevatorPassenger.TargetFloor;
                }
            }
            else if (Math.Abs(ev.DistanceFromHere(nearestQueuedPassenger.StartFloor)) <
                     Math.Abs(ev.DistanceFromHere(nearestInElevatorPassenger.TargetFloor)))
            {
                if (ev.TargetFloor != nearestQueuedPassenger.StartFloor)
                {
                    Debug.Log($"Target changed from2 {ev.TargetFloor.FloorIdx} -> {nearestQueuedPassenger.StartFloor}");
                    ev.TargetFloor = nearestQueuedPassenger.StartFloor;
                }
            }
            else
            {
                if (ev.TargetFloor != nearestQueuedPassenger.StartFloor)
                {
                    Debug.Log($"Target changed from3 {ev.TargetFloor.FloorIdx} -> {nearestQueuedPassenger.StartFloor}");
                    ev.TargetFloor = nearestQueuedPassenger.StartFloor;
                }
            }
        }

        if (nearestQueuedPassenger != null && nearestInElevatorPassenger == null)
        {
            if (ev.TargetFloor != nearestQueuedPassenger.StartFloor)
            {
                Debug.Log($"Target changed from4 {ev.TargetFloor.FloorIdx} -> {nearestQueuedPassenger.StartFloor}");
                ev.TargetFloor = nearestQueuedPassenger.StartFloor;
            }
        }

        if (nearestQueuedPassenger == null && nearestInElevatorPassenger != null)
        {
            if (ev.TargetFloor != nearestInElevatorPassenger.TargetFloor)
            {
                Debug.Log(
                    $"Target changed from5 {ev.TargetFloor.FloorIdx} -> {nearestInElevatorPassenger.TargetFloor}");
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
                elevator._previousFloor == floor &&
                elevator.CurrentState == ElevatorController.ElevatorState.ONBOARDING &&
                (elevator.CurrentDirection == ElevatorDirection.UNWARE || elevator.CurrentDirection == dir)
            )
                return elevator;
        }

        if (true)
        {
            Debug.Log("HERE");
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
            int diff1 = Math.Abs(p1.StartFloor.FloorIdx - e.Elevator._previousFloor.FloorIdx);
            int diff2 = Math.Abs(p2.StartFloor.FloorIdx - e.Elevator._previousFloor.FloorIdx);
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

    public bool IsQueueEmpty(ElevatorController ev)
    {
        return PassengerManager.Instance.GetQueuedPassengers(ev).Count == 0;
    }

    public void SetElevatorCapacity(int idx, int maxCapacity)
    {
        _elevators[idx].ChangeCapacity(maxCapacity);
    }

    public void SetEmptyWeight(int idx, float emptyWeight)
    {
        _elevators[idx].ChangeEmptyWeight(emptyWeight);
    }
}