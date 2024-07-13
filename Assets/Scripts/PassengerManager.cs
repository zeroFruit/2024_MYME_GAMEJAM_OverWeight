using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;

public class PassengerManager : Singleton<PassengerManager>, EventListener<ElevatorPassengerEnteredEvent>
{
    private List<Passenger> _passengers;

    public Passenger Spawn(Floor floor) // todo chagne it to customer
    {
        // prefab 에서 하나 만들고
        // Customer go = Object.Instantiate<Customer>();
        // Passenger created = go.AddComponent<Passenger>();
        // _passengers.Add(created);
        // return go;
        return null;
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