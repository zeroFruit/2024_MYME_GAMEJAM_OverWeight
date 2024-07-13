using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public int FloorIdx { get; set; }
    public int capacityOfPassengers;
    private List<Passenger> _passengers;
    
    private void Awake()
    {
        FloorIdx = 0;
        capacityOfPassengers = 10;
        _passengers = new List<Passenger>();
    }
    
    public void SpawnCustomer(Passenger passenger)
    {
        _passengers.Add(passenger);
    }

    public List<Passenger> GetPassengersToOnboard(int remainWeight, ElevatorDirection afterDirection)
    {
        
        throw new NotImplementedException();
    }

    public void OnboardPassenger(Passenger passenger)
    {
        _passengers.Remove(passenger);
    }

    public static ElevatorDirection operator -(Floor from, Floor to)
    {
        if (from.FloorIdx > to.FloorIdx)
        {
            return ElevatorDirection.Down;
        }
        else if (from.FloorIdx < to.FloorIdx)

        {
            return ElevatorDirection.Up;
        }

        return ElevatorDirection.UNWARE;
    }

    public static bool operator >(Floor left, Floor right)
    {
        return left.FloorIdx > right.FloorIdx;
    }

    public static bool operator <(Floor left, Floor right)
    {
        return left.FloorIdx < right.FloorIdx;
    }
}