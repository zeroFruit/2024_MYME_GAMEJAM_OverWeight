using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FloorManager : Singleton<FloorManager>, EventListener<ElevatorArrivalEvent>,
    EventListener<ElevatorPassengerEnteredEvent>, EventListener<DayEvent>
{
    public List<Floor> Floors { get; private set; }

    [Header("Game")] public int day; // temp

    public bool isPlaying;

    [Header("Spawn")] public float spawnTimer;
    public float spawnSpeed;
    public float spawnTiming;
    
    [Header("SpawnProbability")] public int normalSpawnProbability;
    public int onWorkSpawnProbability;
    public int lunchSpawnProbability;
    public int offWorkSpawnProbability;

    [Header("Prefab")] public Floor floorPrefab;
    public Slot slotPrefab;
    public FloorTimer FloorTimerPrefab;

    protected override void Awake()
    {
        base.Awake();
        Floors = new List<Floor>();
        day = 14;
        isPlaying = false;
        spawnTimer = 0;
        spawnSpeed = 3f;
        spawnTiming = 10;
        normalSpawnProbability = 20;
        onWorkSpawnProbability = 70;
        lunchSpawnProbability = 40;
        offWorkSpawnProbability = 70;
        for (int idx = 0; idx < day; idx++)
        {
            Floor floor = Instantiate(floorPrefab);
            floor.name += $"{idx}";
            floor.Init(slotPrefab, idx, FloorTimerPrefab);
            Floors.Add(floor.GetComponent<Floor>());
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            spawnTimer += Time.deltaTime * spawnSpeed;
            if (spawnTimer >= spawnTiming)
            {
                spawnTimer = 0;
                SpawnPassenger();
            }
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
        List<Passenger> passengersToOnboard = e.ArrivalFloor.GetPassengersToOnboard(e.Elevator,e.RemainWeight, e.AfterDirection);
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


    public void OnEvent(DayEvent e)
    {
        switch (e.EventType)
        {
            case DayEventType.DayStarted:
                this.isPlaying = true;
                break;
            case DayEventType.DayEnded:
                this.isPlaying = false;
                this.reset();
                break;
            case DayEventType.WaveStarted:
                this.ApplyWave((WaveType)e.Args);
                break;
            case DayEventType.WaveEnded:
                this.ApplyWave(WaveType.None);
                break;
            default:
                Debug.Log("FloorManager: Unhandled Event Type! - " + e.EventType.ToString());
                break;
        }

        this.isPlaying = true;
    }

    private void ApplyWave(WaveType waveType)
    {
        foreach (var floor in Floors)
        {
            floor.changeWave(waveType);
        }
    }

    private void ResetWave(WaveType waveType)
    {
        throw new NotImplementedException();
    }

    private void reset()
    {
        foreach (var floor in Floors)
        {
            floor.ResetFloor();
        }
    }

    void OnEnable()
    {
        this.StartListeningEvent<ElevatorArrivalEvent>();
        this.StartListeningEvent<ElevatorPassengerEnteredEvent>();
        this.StartListeningEvent<DayEvent>();
    }

    void OnDisable()
    {
        this.StopListeningEvent<ElevatorArrivalEvent>();
        this.StopListeningEvent<ElevatorPassengerEnteredEvent>();
        this.StopListeningEvent<DayEvent>();
    }

    #region Debug

    [InspectorButton("TestRemovePassenger")]
    public bool TestRemovePassengerButton;

    void TestRemovePassenger()
    {
        ElevatorPassengerEnteredEvent.Trigger(PassengerManager.Instance.Passengers.First());
    }

    #endregion
}