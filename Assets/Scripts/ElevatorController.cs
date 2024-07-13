using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class ElevatorController : MonoBehaviour
{
    public float _speed = 3.0f;
    public float _accelerationThreshold = 1f;
    public float _decelerationThreshold = 1f;
    public int _maxCapacity;
    public List<Passenger> Passengers;
    public List<TextMeshPro> PassengerInfoText;
    public List<BoxScript> BoxScripts;
    public List<Floor> StoppableFloors;
    public Floor TargetFloor;
    public Floor _previousFloor;
    public ElevatorState CurrentState;
    public ElevatorDirection CurrentDirection;

    private void Start()
    {
    }

    public void Init(
        List<Floor> stoppableFloors,
        Floor lobbyFloor,
        int maxCapacity)
    {
        StoppableFloors = stoppableFloors;
        Passengers = new List<Passenger>();
        CurrentDirection = ElevatorDirection.UNWARE;
        CurrentState = ElevatorState.IDLE;
        TargetFloor = lobbyFloor;
        _previousFloor = lobbyFloor;
        _maxCapacity = maxCapacity;

        BoxScripts = new List<BoxScript>(gameObject.GetComponentsInChildren<BoxScript>());
        foreach (var boxScript in BoxScripts)
        {
            if (boxScript.Idx > maxCapacity - 1)
            {
                boxScript.gameObject.SetActive(false);
            }
        }
    }

    public enum ElevatorState
    {
        IDLE,
        MOVING,
        OFFBOARDING,
        ONBOARDING,
    }

    void Update()
    {
        switch (CurrentState)
        {
            case ElevatorState.IDLE:
                UpdateIdle();
                break;
            case ElevatorState.MOVING:
                UpdateMoving();
                break;
            case ElevatorState.OFFBOARDING:
                UpdateOffBoarding();
                break;
            case ElevatorState.ONBOARDING:
                UpdateOnBoarding();
                break;
        }
    }

    private void UpdateOnBoarding()
    {
        Debug.Log($"Start onboarding {_previousFloor.FloorIdx}-{TargetFloor.FloorIdx}");
        // 에니메이션이나 뭐 하면될듯 ?
        if (ElevatorManager.Instance.IsQueueEmpty(this) && Passengers.Count == 0)
        {
            CurrentDirection = ElevatorDirection.UNWARE;
        }

        ElevatorArrivalEvent.Trigger(
            TargetFloor,
            _maxCapacity - CurrentCapacity(),
            CurrentDirection,
            this
        );

        // n초 대기후 상태변경해주면 될듯합니다
        if (Passengers.Count == 0)
        {
            Debug.Log($"onboarding makes to idle");
            CurrentState = ElevatorState.IDLE;
        }
        else
        {
            // todo : passenger가 탄거니까 움직일 층 찾고 이동상태로 만들어야함.
            CurrentState = ElevatorState.MOVING;
            ElevatorManager.Instance.RouteElevator(this);
            if (TargetFloor == _previousFloor)
            {
                Debug.Log($"noooo dont here");
                ElevatorManager.Instance.RouteElevator(this);
            }
        }
    }


    private void UpdateOffBoarding()
    {
        // 에니메이션이나 뭐 하면될듯 ? 몇초기다리기...
        Debug.Log($"Start offboarding {_previousFloor.FloorIdx}->{TargetFloor.FloorIdx}");
        _previousFloor = TargetFloor;
        List<Passenger> exitWantPassengers = GetExitWantPassengers(_previousFloor);
        foreach (var passenger in exitWantPassengers)
        {
            Exit(passenger);
        }

        CurrentState = ElevatorState.ONBOARDING;
    }

    private void UpdateIdle()
    {
        List<Passenger> passengers = PassengerManager.Instance.GetPassengersOnFloor(_previousFloor);
        if (passengers.Count(passenger => passenger.queuedElevator == this) != 0)
        {
            CurrentDirection = ElevatorDirection.UNWARE;
            CurrentState = ElevatorState.ONBOARDING;
        }

        if (_previousFloor != TargetFloor)
        {
            CurrentState = ElevatorState.MOVING;
            CurrentDirection = _previousFloor.DirectionTo(TargetFloor);
            Debug.Log("change to MOVING");
        }
    }

    private void UpdateMoving()
    {
        float currentPosition = transform.position.y;
        float startPosition = _previousFloor.transform.position.y;
        float targetPosition = TargetFloor.transform.position.y;
        float diff = CurrentDirection.ToFloat() * _speed * Time.deltaTime;
        float updatedPosition = currentPosition + diff;

        // 넘어가는거 보정
        if ((CurrentDirection == ElevatorDirection.Up && updatedPosition >= targetPosition) ||
            (CurrentDirection == ElevatorDirection.Down && updatedPosition <= targetPosition))
        {
            updatedPosition = targetPosition;
        }

        // _currentFloor = _targetFloor;
        // _elevatorState = ElevatorState.IDLE;
        if (Math.Abs(currentPosition - startPosition) < _accelerationThreshold)
        {
            float lerpRatio = Math.Max(0.01f, Math.Abs(currentPosition - startPosition) / _accelerationThreshold);
            updatedPosition = Mathf.Lerp(currentPosition, updatedPosition, lerpRatio);
        }
        else if (Math.Abs(currentPosition - targetPosition) < _decelerationThreshold)
        {
            float lerpRatio = Math.Max(0.01f, Math.Abs(currentPosition - targetPosition) / _decelerationThreshold);
            updatedPosition = Mathf.Lerp(currentPosition, updatedPosition, lerpRatio);
        }

        if (Math.Abs(currentPosition - targetPosition) < 0.01f)
        {
            updatedPosition = targetPosition;
            CurrentState = ElevatorState.OFFBOARDING;
        }

        transform.position = new Vector3(transform.position.x, updatedPosition);
    }

    public bool Enter(Passenger passenger)
    {
        if (!CanEnterCapacity(passenger))
        {
            Debug.Log(
                $"enter fail because full capacity : ${passenger.StartFloor.FloorIdx} : ${passenger.GetInstanceID()}");
            return false;
        }

        if (CurrentState != ElevatorState.ONBOARDING)
        {
            Debug.Log(
                $"enter fail because not onboarding : ${passenger.StartFloor.FloorIdx} : ${passenger.GetInstanceID()}");
            return false;
        }

        passenger.inElevator = true;
        if (Passengers.Count == 0)
        {
            CurrentDirection = passenger.StartFloor.DirectionTo(passenger.TargetFloor);
        }

        Passengers.Add(passenger);
        // todo : 사람 표시
        foreach (var boxScript in BoxScripts)
        {
            if (!boxScript.IsEmpty())
            {
                continue;
            }

            boxScript.SetText(passenger.TargetFloor.FloorIdx.ToString());
            break;
        }

        // 수정필요
        Debug.Log($"enter passenger : ${passenger.StartFloor.FloorIdx} : ${passenger.GetInstanceID()}");
        ElevatorPassengerEnteredEvent.Trigger(passenger);
        return true;
    }

    private void Exit(Passenger passenger)
    {
        Passengers.Remove(passenger);
        Debug.Log(
            $"exit : {passenger.StartFloor.FloorIdx} -> {passenger.StartFloor.FloorIdx} : ${passenger.GetInstanceID()}");
        // todo : 사람 표시 제거
        foreach (var boxScript in BoxScripts)
        {
            if (boxScript.IsText(passenger.TargetFloor.FloorIdx.ToString()))
            {
                boxScript.MakeEmpty();
                break;
            }
        }

        ElevatorPassengerExitEvent.Trigger(passenger);
    }

    private List<Passenger> GetExitWantPassengers(Floor floor)
    {
        List<Passenger> exitWantPassengers = new List<Passenger>();
        foreach (var passenger in Passengers)
        {
            if (passenger.TargetFloor == floor)
            {
                exitWantPassengers.Add(passenger);
            }
        }

        return exitWantPassengers;
    }

    public bool CanEnterCapacity(Passenger passenger)
    {
        return RemainCapacity() >= passenger.Weight;
    }

    public bool CanStop(Floor floor)
    {
        if (CurrentState == ElevatorState.IDLE)
        {
            // Debug.Log("can stop check with unware");
            return true;
        }

        if (CurrentDirection == ElevatorDirection.Up)
        {
            if (floor.transform.position.y - transform.position.y > _decelerationThreshold)
                return true;
            return false;
        }

        if (CurrentDirection == ElevatorDirection.Down)
        {
            if (transform.position.y - floor.transform.position.y > _decelerationThreshold)
                return true;
            return false;
        }

        Debug.Log("why can stop check here?");
        return false;
    }

    public bool IsFull()
    {
        return CurrentCapacity() == _maxCapacity;
    }

    public int CurrentCapacity()
    {
        int totalWeight = 0;
        foreach (var passenger in Passengers)
        {
            totalWeight += passenger.Weight;
        }

        return totalWeight;
    }

    public int RemainCapacity()
    {
        return _maxCapacity - CurrentCapacity();
    }

    public float DistanceFromHere(Floor floor)
    {
        return this.transform.position.y - floor.transform.position.y;
    }

    public bool IsStoppable(Floor floor)
    {
        return StoppableFloors.Contains(floor);
    }

    public bool IsStoppable(Floor from, Floor to)
    {
        return IsStoppable(from) && IsStoppable(to);
    }
}