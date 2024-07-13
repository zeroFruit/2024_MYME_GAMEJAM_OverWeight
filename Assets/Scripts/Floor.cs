using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public FloorTimer timer;
    
    
    public void Init(Slot slotPrefab, int floorIdx, FloorTimer floorTimer)
    {
        FloorIdx = floorIdx;
        capacityOfPassengers = 7;
        maxCapacityOfPassengers = 10;
        spawnProbability = 100;
        Slots = new List<Slot>();
        for (int idx = 0; idx < maxCapacityOfPassengers; idx++)
        {
            Slot slot = Instantiate(slotPrefab, transform);
            slot.Init(floorIdx, idx);
            Slots.Add(slot);
        }

        Passengers = new List<Passenger>();
        timer = Instantiate(floorTimer, transform);
        timer.Init();
    }

    public void Start()
    {
        FloorUiData uiData = FloorUiData.GetFloorUiData(FloorIdx);
        transform.localPosition = new Vector3(uiData.localPosX, uiData.localPosY, 0);
    }

    public void SpawnPassenger()
    {
        if (Passengers.Count >= capacityOfPassengers)
        {
            timer.Progress(10f);
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

    public List<Passenger> GetPassengersToOnboard(ElevatorController elevator, int remainWeight,
        ElevatorDirection afterDirection)
    {
        int usedWeight = 0;
        List<Passenger> passengersToOnboard = new List<Passenger>();
        foreach (var passenger in Passengers)
        {
            ElevatorDirection passengerDirection = passenger.StartFloor.DirectionTo(passenger.TargetFloor);
            if (!elevator.IsStoppable(passenger.StartFloor, passenger.TargetFloor))
            {
                continue;
            }
            if (afterDirection != ElevatorDirection.UNWARE && passengerDirection != afterDirection)
            {
                continue;
            }

            if (usedWeight + passenger.Weight <= remainWeight)
            {
                usedWeight += passenger.Weight;
                afterDirection = passengerDirection; // 첫번째 사람으로 direction 고정
                passengersToOnboard.Add(passenger);
            }
        }

        return passengersToOnboard;
    }

    public void OnboardPassenger(Passenger passenger)
    {
        Passengers.Remove(passenger);
        RearrangePassengers();
        if (Passengers.Count() < capacityOfPassengers)
        {
            timer.ResetProgress();
        }
    }

    private void RearrangePassengers()
    {
        foreach (var slot in Slots)
        {
            Passenger child = slot.GetComponentInChildren<Passenger>();
            if (child != null)
            {
                slot.GetComponentInChildren<Passenger>().transform.SetParent(null);
            }
        }

        for (int idx = 0; idx < Passengers.Count; idx++)
        {
            Passengers[idx].transform.SetParent(Slots[idx].transform);
        }
    }

    public ElevatorDirection DirectionTo(Floor to)
    {
        if (FloorIdx > to.FloorIdx)
        {
            return ElevatorDirection.Down;
        }

        if (FloorIdx < to.FloorIdx)

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