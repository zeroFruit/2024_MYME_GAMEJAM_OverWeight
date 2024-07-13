using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int FloorIdx;
    public int capacityOfPassengers;
    public int maxCapacityOfPassengers;
    public int spawnProbability;
    public List<Slot> Slots;
    public List<Passenger> Passengers;
    
    public void Init(Slot slotPrefab, int floorIdx)
    {
        FloorIdx = floorIdx;
        capacityOfPassengers = 7;
        maxCapacityOfPassengers = 10;
        spawnProbability = 100;
        Slots = new List<Slot>();
        for (int idx = 0; idx < capacityOfPassengers; idx++)
        {
            Slot slot = Instantiate(slotPrefab, transform);
            slot.Init(floorIdx, idx);
            Slots.Add(slot);
        }
        Passengers = new List<Passenger>();
    }

    public void Start()
    {
        FloorUiData uiData = FloorUiData.GetFloorUiData(FloorIdx);
        transform.localPosition = new Vector3(uiData.localPosX, uiData.localPosY, 0);
        transform.localScale = new Vector3(uiData.scaleX, uiData.scaleY, 1);
    }

    public void SpawnPassenger()
    {
        if (Passengers.Count >= capacityOfPassengers)
        {
            Debug.Log("Floor Timer Start!!!");
        }

        if (Passengers.Count < maxCapacityOfPassengers)
        {
            if (isSpawnedRandomly())
            {
                Passenger newPassenger = PassengerManager.Instance.Spawn(this);
                newPassenger.transform.SetParent(getEmptySlot().transform);
                Passengers.Add(newPassenger);
            }
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