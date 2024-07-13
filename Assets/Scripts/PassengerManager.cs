using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;

public class PassengerManager : Singleton<PassengerManager>, EventListener<ElevatorPassengerEnteredEvent>
{
    private List<Passenger> _passengers;

    public Passenger passengerPrefab;

    protected override void Awake()
    {
        base.Awake();
        _passengers = new List<Passenger>();
    }

    public Passenger Spawn(Floor floor)
    {
        Passenger spawned = Instantiate(passengerPrefab);
        spawned.Init();
        _passengers.Add(spawned);
        return spawned;
    }

    void Despawn(Passenger passenger)
    {
        _passengers.Remove(passenger);
        passenger.gameObject.SetActive(false);
    }

    public List<Passenger> GetPassengersOnFloor(Floor floor)
    {
        List<Passenger> result = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            if (floor == passenger.StartFloor)
            {
                result.Add(passenger);
            } 
        }
        return result;
    }
    
    public void OnEvent(ElevatorPassengerEnteredEvent e)
    {
        e.Passenger.gameObject.SetActive(false);
    }

    public List<Passenger> GetMinimalSizePassengerPerFloor()
    {
        throw new System.NotImplementedException();
    }
}