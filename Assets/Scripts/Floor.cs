using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int FloorIdx { get; set; }
    public int capacityOfPassengers;
    public int spawnProbability;
    public List<Slot> Slots;
    public List<Passenger> Passengers;
    
    public void Init()
    {
        FloorIdx = 0;
        capacityOfPassengers = 10;
        spawnProbability = 100;
        Slots = new List<Slot>();
        Passengers = new List<Passenger>();
    }
    
    public void SpawnPassenger()
    {
        if (isSpawnedRandomly())
        {
            Passenger newPassenger = PassengerManager.Instance.Spawn(this);
            newPassenger.transform.SetParent(getEmptySlot().transform);
            Passengers.Add(newPassenger);
        }
    }

    private Slot getEmptySlot()
    {
        return Slots[Passengers.Count];
    }

    private bool isSpawnedRandomly()
    {
        int probability = Random.Range(0, spawnProbability);
        return probability <= capacityOfPassengers;
    }

    public List<Passenger> GetPassengersToOnboard(int remainWeight, ElevatorDirection afterDirection)
    {
        throw new NotImplementedException();
    }

    public void OnboardPassenger(Passenger passenger)
    {
        
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