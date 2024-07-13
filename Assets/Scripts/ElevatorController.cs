using System;
using System.Collections;
using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.Serialization;

public class ElevatorController : MonoBehaviour
{
    private float _speed = 3.0f;
    private float _accelerationThreshold = 1f;
    private float _decelerationThreshold = 1f;
    private int _maxWeight;
    public List<Passenger> Passengers { get; private set; } = new List<Passenger>();
    public Floor TargetFloor { get; set; }
    public Floor CurrentFloor { get; private set; }
    private Floor _previousFloor;
    public ElevatorState CurrentState { get; private set; }

    private void Awake()
    {
        // for test
        Init(GameObject.Find("Floor_0").GetComponent<Floor>(), 100);
        TargetFloor = GameObject.Find("Floor_3").GetComponent<Floor>();
    }

    public void Init(Floor lobbyFloor, int maxWeight)
    {
        CurrentState = ElevatorState.IDLE;
        CurrentFloor = lobbyFloor;
        TargetFloor = lobbyFloor;
        _previousFloor = lobbyFloor;
        _maxWeight = maxWeight;
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
            Passengers.Count > 0 ? _previousFloor - CurrentFloor : ElevatorDirection.UNWARE;
        ElevatorArrivalEvent.Trigger(
            CurrentFloor,
            _previousFloor,
            _maxWeight - GetCurrentWeight(),
            moveDirection
        );

        if (Passengers.Count == 0)
        {
            CurrentState = ElevatorState.IDLE;
        }
        else
        {
            // todo : passenger가 탄거니까 움직일 층 찾고 이동상태로 만들어야함.
            ElevatorManager.Instance.routeElevator(this, moveDirection);
            CurrentState = ElevatorState.MOVING;
        }
    }

    public ElevatorDirection AfterDirection()
    {
        switch (CurrentState)
        {
            case ElevatorState.MOVING:
                return TargetFloor - CurrentFloor;
            case ElevatorState.IDLE:
                return ElevatorDirection.UNWARE;
            case ElevatorState.OFFBOARDING:
            case ElevatorState.ONBOARDING:
                return Passengers.Count > 0 ? _previousFloor - CurrentFloor : ElevatorDirection.UNWARE;
        }

        return ElevatorDirection.UNWARE;
    }

    private void UpdateOffBoarding()
    {
        // 에니메이션이나 뭐 하면될듯 ?
        List<Passenger> exitWantPassengers = GetExitWantPassengers(CurrentFloor);
        foreach (var passenger in exitWantPassengers)
        {
            Exit(passenger);
        }

        CurrentState = ElevatorState.ONBOARDING;
    }

    private void UpdateIdle()
    {
        if (CurrentFloor != TargetFloor)
        {
            CurrentState = ElevatorState.MOVING;
            Debug.Log("change to MOVING");
        }
    }

    private void UpdateMoving()
    {
        ElevatorDirection direction = TargetFloor.transform.position.y > CurrentFloor.transform.position.y
            ? ElevatorDirection.Up
            : ElevatorDirection.Down;
        float currentPosition = transform.position.y;
        float startPosition = CurrentFloor.transform.position.y;
        float targetPosition = TargetFloor.transform.position.y;
        float diff = direction.ToFloat() * _speed * Time.deltaTime;
        float updatedPosition = currentPosition + diff;

        // 넘어가는거 보정
        if ((direction == ElevatorDirection.Up && updatedPosition >= targetPosition) ||
            (direction == ElevatorDirection.Down && updatedPosition <= targetPosition))
        {
            Debug.Log("보정");
            updatedPosition = targetPosition;
        }

        // _currentFloor = _targetFloor;
        // _elevatorState = ElevatorState.IDLE;
        if (Math.Abs(currentPosition - startPosition) < _accelerationThreshold)
        {
            Debug.Log(Math.Abs(currentPosition - startPosition));
            Debug.Log(_accelerationThreshold);
            Debug.Log("accelation Threshold!");
            float lerpRatio = Math.Max(0.01f, Math.Abs(currentPosition - startPosition) / _accelerationThreshold);
            updatedPosition = Mathf.Lerp(currentPosition, updatedPosition, lerpRatio);
        }
        else if (Math.Abs(currentPosition - targetPosition) < _decelerationThreshold)
        {
            Debug.Log("decelation Threshold!");
            float lerpRatio = Math.Max(0.01f, Math.Abs(currentPosition - targetPosition) / _decelerationThreshold);
            updatedPosition = Mathf.Lerp(currentPosition, updatedPosition, lerpRatio);
        }
        else
        {
            Debug.Log("just moving!");
        }

        if (Math.Abs(currentPosition - targetPosition) < 0.01f)
        {
            updatedPosition = targetPosition;
            _previousFloor = CurrentFloor;
            CurrentFloor = TargetFloor;
            CurrentState = ElevatorState.OFFBOARDING;
        }

        transform.position = new Vector3(transform.position.x, updatedPosition);
    }

    public bool Enter(Passenger passenger)
    {
        if (!CanEnter(passenger)) return false;
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

    public bool CanEnter(Passenger passenger)
    {
        int remainWeight = _maxWeight - GetCurrentWeight();
        return remainWeight > passenger.Weight;
    }

    public int GetCurrentWeight()
    {
        int totalWeight = 0;
        foreach (var passenger in Passengers)
        {
            totalWeight += passenger.Weight;
        }

        return totalWeight;
    }
}