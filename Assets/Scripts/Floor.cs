using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public int floorIdx { get; set; }
    public int capacityOfPassengers;
    private List<Passenger> _passengers;
    
    private void Awake()
    {
        floorIdx = 0;
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
}