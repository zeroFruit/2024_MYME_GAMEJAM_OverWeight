using System;
using System.Collections.Generic;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FloorManager : Singleton<FloorManager>, EventListener<ElevatorArrivalEvent>,
    EventListener<ElevatorPassengerEnteredEvent>
{
    private List<Floor> _floors;
    public int day; // temp

    [Header("Spawn")] public float spawnTimer;
    public float spawnSpeed;
    public float spawnTiming;

    [Header("Prefab")]
    public Floor floorPrefab;
    public Slot slotPrefab;

    protected override void Awake()
    {
        base.Awake();
        _floors = new List<Floor>();
        day = 14;
        spawnTimer = 0;
        spawnSpeed = 3f;
        spawnTiming = 10;
        for (int idx = 0; idx < day; idx++)
        {
            Floor floor = Instantiate(floorPrefab);
            floor.Init(slotPrefab, idx);
            _floors.Add(floor.GetComponent<Floor>());
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
        foreach (var floor in _floors)
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
            selected = _floors[Random.Range(0, _floors.Count)];
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
}