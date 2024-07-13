using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FloorManager : Singleton<FloorManager>, EventListener<ElevatorArrivalEvent>,
    EventListener<ElevatorPassengerEnteredEvent>, EventListener<DayEvent>, EventListener<GameOverEvent>
{
    public List<Floor> Floors { get; private set; }

    public int maxFloorNum;
    public int initialFloorNum;

    public bool isPlaying;

    [Header("Spawn")] public float spawnTimer;
    public float spawnDuration;
    
    [Header("SpawnProbability")] public int normalSpawnProbability;
    public int onWorkSpawnProbability;
    public int lunchSpawnProbability;
    public int offWorkSpawnProbability;

    [Header("Prefab")] public Floor floorPrefab;
    public Slot slotPrefab;
    public FloorTimer FloorTimerPrefab;

    [Header("UIPaenl")] public UIPanel GameOverPanel;

    protected override void Awake()
    {
        base.Awake();
        Floors = new List<Floor>();
        maxFloorNum = 14;
        initialFloorNum = 6;
        isPlaying = false;
        spawnTimer = 0;
        spawnDuration = 10f;
        normalSpawnProbability = 20;
        onWorkSpawnProbability = 70;
        lunchSpawnProbability = 40;
        offWorkSpawnProbability = 70;
        for (int idx = 0; idx < maxFloorNum; idx++)
        {
            Floor floor = Instantiate(floorPrefab);
            floor.name += $"{idx}";
            floor.Init(slotPrefab, idx, "블록체인 연구소", FloorTimerPrefab);
            Floors.Add(floor.GetComponent<Floor>());
            if (floor.FloorIdx < initialFloorNum)
            {
                floor.Actiavte();
            }
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDuration)
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
        PassengerManager.Instance.Passengers.Remove(e.Passenger);
        e.Passenger.gameObject.SetActive(false);
    }

    public Floor GetRandomFloor(Floor without)
    {
        Floor selected = without;
        while (selected == without)
        {
            selected = Floors[Random.Range(0, GetNowFloorNum())];
        }

        return selected;
    }


    public void OnEvent(DayEvent e)
    {
        switch (e.EventType)
        {
            case DayEventType.DayStarted:
                foreach (var floor in Floors)
                {
                    if (floor.FloorIdx < GetNowFloorNum())
                    {
                        floor.Actiavte();   
                    }
                }
                this.isPlaying = true;
                break;
            case DayEventType.DayEnded:
                this.isPlaying = false;
                break;
            case DayEventType.WaveStarted:
                this.ApplyWave((WaveType)e.Args);
                break;
            case DayEventType.WaveEnded:
                this.ApplyWave(WaveType.None);
                break;
            default:
                // Debug.Log("FloorManager: Unhandled Event Type! - " + e.EventType.ToString());
                break;
        }
    }

    public int GetNowFloorNum()
    {
        return initialFloorNum + DayManager.Instance.Day;
    }

    private void ApplyWave(WaveType waveType)
    {
        foreach (var floor in Floors)
        {
            floor.changeWave(waveType);
        }
    }

    public void OnEvent(GameOverEvent e)
    {
        Debug.Log("Game Over!!");
        this.isPlaying = false;
        foreach (var floor in Floors)
        {
            floor.StopFloor();
        }
        GameOverPanel.Show(null);
    }


    void OnEnable()
    {
        this.StartListeningEvent<ElevatorArrivalEvent>();
        this.StartListeningEvent<ElevatorPassengerEnteredEvent>();
        this.StartListeningEvent<DayEvent>();
        this.StartListeningEvent<GameOverEvent>();
    }

    void OnDisable()
    {
        this.StopListeningEvent<ElevatorArrivalEvent>();
        this.StopListeningEvent<ElevatorPassengerEnteredEvent>();
        this.StopListeningEvent<DayEvent>();
        this.StopListeningEvent<GameOverEvent>();
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