using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FloorManager : Singleton<FloorManager>, EventListener<ElevatorArrivalEvent>,
    EventListener<ElevatorPassengerEnteredEvent>
{
    public List<Floor> Floors { get; private set; }
    public int day; // temp

    [Header("Spawn")] public float spawnTimer;
    public float spawnSpeed;
    public float spawnTiming;

    [Header("Prefab")] public Floor floorPrefab;
    public Slot slotPrefab;
    public FloorTimer FloorTimerPrefab;

    protected override void Awake()
    {
        base.Awake();
        Floors = new List<Floor>();
        day = 14;
        spawnTimer = 0;
        spawnSpeed = 3f;
        spawnTiming = 10;
        for (int idx = 0; idx < day; idx++)
        {
            Floor floor = Instantiate(floorPrefab);
            floor.Init(slotPrefab, idx, FloorTimerPrefab);
            Floors.Add(floor.GetComponent<Floor>());
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime * spawnSpeed;
        if (spawnTimer >= spawnTiming)
        {
            spawnTimer = 0;
            SpawnPassenger();
        }
    }

    private void SpawnPassenger()
    {
        foreach (var floor in Floors)
        {
            floor.SpawnPassenger();
        }
    }

    public void OnEvent(ElevatorArrivalEvent e)
    {
        List<Passenger> passengersToOnboard = e.ArrivalFloor.GetPassengersToOnboard(e.RemainWeight, e.AfterDirection);
        foreach (Passenger passenger in passengersToOnboard)
        {
            ElevatorManager.Instance.Enter(passenger);
        }
    }

    public void OnEvent(ElevatorPassengerEnteredEvent e)
    {
        Floor floor = e.Passenger.StartFloor;
        floor.OnboardPassenger(e.Passenger);
    }

    public Floor GetRandomFloor(Floor without)
    {
        Floor selected = without;
        while (selected == without)
        {
            selected = Floors[Random.Range(0, Floors.Count)];
        }

        return selected;
    }

    void OnEnable()
    {
        this.StartListeningEvent<ElevatorArrivalEvent>();
        this.StartListeningEvent<ElevatorPassengerEnteredEvent>();
    }

    void OnDisable()
    {
        this.StopListeningEvent<ElevatorArrivalEvent>();
        this.StopListeningEvent<ElevatorPassengerEnteredEvent>();
    }

    #region Debug
    [InspectorButton("TestRemovePassenger")] public bool TestRemovePassengerButton;

    void TestRemovePassenger()
    {
        ElevatorPassengerEnteredEvent.Trigger(PassengerManager.Instance.Passengers.First());
    }
    #endregion
}