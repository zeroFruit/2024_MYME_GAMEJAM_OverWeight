using System;
using System.Collections;
using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.Serialization;

public class ElevatorController : MonoBehaviour
{
    public float _speed = 3.0f;
    public float _accelerationThreshold = 1f;
    public float _decelerationThreshold = 1f;
    public int _maxCapacity;
    public List<Passenger> Passengers;
    public Floor TargetFloor { get; set; }
    public Floor MoveStartFloor { get; private set; }
    public Floor _previousFloor;
    public ElevatorState CurrentState { get; private set; }

    private void Awake()
    {
        // for test
        Passengers = new List<Passenger>();
        Init(GameObject.Find("Floor_0").GetComponent<Floor>(), 100);
    }

    public void Init(Floor lobbyFloor, int maxCapacity)
    {
        CurrentState = ElevatorState.IDLE;
        MoveStartFloor = lobbyFloor;
        TargetFloor = lobbyFloor;
        _previousFloor = lobbyFloor;
        _maxCapacity = maxCapacity;
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
        // 에니메이션이나 뭐 하면될듯 ?
        ElevatorDirection moveDirection =
            Passengers.Count > 0 ? _previousFloor.DirectionTo(MoveStartFloor) : ElevatorDirection.UNWARE;
        ElevatorArrivalEvent.Trigger(
            MoveStartFloor,
            _previousFloor,
            _maxCapacity - CurrentCapacity(),
            moveDirection
        );

        // n초 대기후 상태변경해주면 될듯합니다
        if (Passengers.Count == 0)
        {
            CurrentState = ElevatorState.IDLE;
        }
        else
        {
            // todo : passenger가 탄거니까 움직일 층 찾고 이동상태로 만들어야함.
            CurrentState = ElevatorState.MOVING;
            ElevatorManager.Instance.RouteElevator(this);
        }
    }

    public ElevatorDirection AfterDirection()
    {
        switch (CurrentState)
        {
            case ElevatorState.MOVING:
                return MoveStartFloor.DirectionTo(TargetFloor);
            case ElevatorState.IDLE:
                return ElevatorDirection.UNWARE;
            case ElevatorState.OFFBOARDING:
            case ElevatorState.ONBOARDING:
                return Passengers.Count > 0 ? _previousFloor.DirectionTo(MoveStartFloor) : ElevatorDirection.UNWARE;
        }

        return ElevatorDirection.UNWARE;
    }

    private void UpdateOffBoarding()
    {
        // 에니메이션이나 뭐 하면될듯 ? 몇초기다리기...
        List<Passenger> exitWantPassengers = GetExitWantPassengers(MoveStartFloor);
        foreach (var passenger in exitWantPassengers)
        {
            Exit(passenger);
        }

        CurrentState = ElevatorState.ONBOARDING;
    }

    private void UpdateIdle()
    {
        if (MoveStartFloor != TargetFloor)
        {
            CurrentState = ElevatorState.MOVING;
            Debug.Log("change to MOVING");
        }
    }

    private void UpdateMoving()
    {
        ElevatorDirection direction = TargetFloor.transform.position.y > MoveStartFloor.transform.position.y
            ? ElevatorDirection.Up
            : ElevatorDirection.Down;
        float currentPosition = transform.position.y;
        float startPosition = MoveStartFloor.transform.position.y;
        float targetPosition = TargetFloor.transform.position.y;
        float diff = direction.ToFloat() * _speed * Time.deltaTime;
        float updatedPosition = currentPosition + diff;

        // 넘어가는거 보정
        if ((direction == ElevatorDirection.Up && updatedPosition >= targetPosition) ||
            (direction == ElevatorDirection.Down && updatedPosition <= targetPosition))
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
            _previousFloor = MoveStartFloor;
            MoveStartFloor = TargetFloor;
            CurrentState = ElevatorState.OFFBOARDING;
        }

        transform.position = new Vector3(transform.position.x, updatedPosition);
    }

    public bool Enter(Passenger passenger)
    {
        if (!CanEnterCapacity(passenger)) return false;
        if (CurrentState != ElevatorState.ONBOARDING) return false;
        passenger.inElevator = true;
        Passengers.Add(passenger);
        ElevatorPassengerEnteredEvent.Trigger(passenger);
        return true;
    }

    private void Exit(Passenger passenger)
    {
        Passengers.Remove(passenger);
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
        return RemainCapacity() > passenger.Weight;
    }

    public bool CanStop(Floor floor)
    {
        ElevatorDirection currentMovingDirection = MoveStartFloor.DirectionTo(TargetFloor);
        if (currentMovingDirection == ElevatorDirection.UNWARE)
        {
            Debug.Log("can stop check with unware");
            return true;
        }

        if (currentMovingDirection == ElevatorDirection.Up)
        {
            if (floor.transform.position.y - transform.position.y > _decelerationThreshold)
                return true;
            return false;
        }

        if (currentMovingDirection == ElevatorDirection.Down)
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
}