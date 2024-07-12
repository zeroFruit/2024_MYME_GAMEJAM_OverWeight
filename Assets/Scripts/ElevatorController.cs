using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private float _speed = 3.0f;
    private float _accelerationThreshold = 1f;
    private float _decelerationThreshold = 1f;
    private int _maxWeight;
    private List<Passenger> _passengers;
    [SerializeField] private Floor _targetFloor;
    [SerializeField] private Floor _currentFloor;

    private ElevatorState _elevatorState = ElevatorState.IDLE;

    enum ElevatorState
    {
        IDLE,
        MOVING,
        EXIST_ING,
        ENTER_ING,
    }

    void Start()
    {
    }

    void Update()
    {
        // if (Input.GetKey(1))
        // {
        // }
        switch (_elevatorState)
        {
            case ElevatorState.IDLE:
                UpdateIdle();
                break;
            case ElevatorState.MOVING:
                UpdateMoving();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (_currentFloor != _targetFloor)
        {
            _elevatorState = ElevatorState.MOVING;
            Debug.Log("change to MOVING");
        }
    }

    private void UpdateMoving()
    {
        Direction direction = _targetFloor.transform.position.y > _currentFloor.transform.position.y
            ? Direction.Up
            : Direction.Down;
        float currentPosition = transform.position.y;
        float startPosition = _currentFloor.transform.position.y;
        float targetPosition = _targetFloor.transform.position.y;
        float diff = direction.ToFloat() * _speed * Time.deltaTime;
        float updatedPosition = currentPosition + diff;

        // 넘어가는거 보정
        if ((direction == Direction.Up && updatedPosition >= targetPosition) ||
            (direction == Direction.Down && updatedPosition <= targetPosition))
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
            _elevatorState = ElevatorState.EXIST_ING;
            _currentFloor = _targetFloor;
        }

        transform.position = new Vector3(transform.position.x, updatedPosition);
    }

    public bool Enter(Passenger passenger)
    {
        if (!CanEnter()) return false;
        _passengers.Add(passenger);
        return true;
    }

    private void Exit(Passenger passenger)
    {
        _passengers.Remove(passenger);
    }

    private List<Passenger> GetExitWantPassengers(Floor floor)
    {
        List<Passenger> exitWantPassengers = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            if (passenger.TargetFloor == floor)
            {
                exitWantPassengers.Add(passenger);
            }
        }

        return exitWantPassengers;
    }

    public bool CanEnter()
    {
        return _passengers.Count < _maxWeight; // todo   weight계산 바꿔야함
    }
}

public enum Direction
{
    Up = 1,
    Down = -1
}

public static class DirectionExtensions
{
    public static float ToFloat(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return 1f;
            case Direction.Down:
                return -1f;
            default:
                return 0f;
        }
    }
}