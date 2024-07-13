using System;
using System.Collections.Generic;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorManager : Singleton<FloorManager>, EventListener<ElevatorArrivalEvent>, EventListener<ElevatorPassengerEnteredEvent>
{
    private List<Floor> _floors;

    protected override void Awake()
    {
        base.Awake();
        _floors = new List<Floor>();
    }

    void SpawnCustomer()
    {
        Floor spawningFloor = GetRandomFloor();
        Passenger passenger = PassengerManager.Instance.Spawn(spawningFloor);
        spawningFloor.SpawnCustomer(passenger);
    }

    private Floor GetRandomFloor()
    {
        int find = Mathf.RoundToInt(Random.Range(0, _floors.Count));
        foreach (Floor floor in _floors)
        {
            if (find == floor.FloorIdx)
            {
                return floor;
            }
        }
        return null;
    }

    public void OnEvent(ElevatorArrivalEvent e)
    {
        List<Passenger> passengersToOnboard = e.ArrivalFloor.GetPassengersToOnboard(e.RemainWeight, e.AfterDirection);
        if (ElevatorManager.Enter(passengersToOnboard))
        {
            
        }
    }

    public void OnEvent(ElevatorPassengerEnteredEvent e)
    {
        Floor floor = e.Passenger.StartFloor;
        floor.OnboardPassenger(e.Passenger);
    }
}