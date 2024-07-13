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

    public List<Passenger> GetAllPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            passengers.Add(passenger);
        }

        return passengers;
    }

    public List<Passenger> GetUnQueuedPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            if (!passenger.inElevator && passenger.queuedElevator == null)
            {
                passengers.Add(passenger);
            }
        }

        return passengers;
    }

    public List<Passenger> GetQueuedPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            if (!passenger.inElevator && passenger.queuedElevator != null)
            {
                passengers.Add(passenger);
            }
        }

        return passengers;
    }

    public List<Passenger> GetQueuedPassengers(ElevatorController ev)
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in _passengers)
        {
            if (!passenger.inElevator && passenger.queuedElevator == ev)
            {
                passengers.Add(passenger);
            }
        }

        return passengers;
    }
    
    void OnEnable()
    {
        this.StartListeningEvent<ElevatorPassengerEnteredEvent>();
    }

    void OnDisable()
    {
        this.StopListeningEvent<ElevatorPassengerEnteredEvent>();
    }
}