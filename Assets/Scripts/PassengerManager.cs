using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.Serialization;

public class PassengerManager : Singleton<PassengerManager>, EventListener<ElevatorPassengerEnteredEvent>
{
    public List<Passenger> Passengers;

    public Passenger passengerPrefab;

    protected override void Awake()
    {
        base.Awake();
        Passengers = new List<Passenger>();
    }

    public Passenger Spawn(Floor start, Floor target)
    {
        Passenger spawned = Instantiate(passengerPrefab);
        spawned.Init(start, target);
        Passengers.Add(spawned);
        return spawned;
    }

    void Despawn(Passenger passenger)
    {
        Passengers.Remove(passenger);
        passenger.gameObject.SetActive(false);
    }

    public List<Passenger> GetPassengersOnFloor(Floor floor)
    {
        List<Passenger> result = new List<Passenger>();
        foreach (var passenger in Passengers)
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
        Passengers.Remove(e.Passenger);
        e.Passenger.gameObject.SetActive(false);
    }

    public List<Passenger> GetMinimalSizePassengerPerFloor()
    {
        throw new System.NotImplementedException();
    }

    public List<Passenger> GetAllPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in Passengers)
        {
            passengers.Add(passenger);
        }

        return passengers;
    }

    public List<Passenger> GetUnQueuedPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        foreach (var passenger in Passengers)
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
        foreach (var passenger in Passengers)
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
        foreach (var passenger in Passengers)
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