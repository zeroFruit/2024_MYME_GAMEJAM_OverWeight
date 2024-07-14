using System;
using System.Collections.Generic;
using System.Linq;
using SSR.OverWeight;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
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
    public float lunchMultiplier;
    public List<int> lunchActiveNumber;
    
    public List<float> daySpawnDuration = new List<float>();
    public float spawnMinDuration;

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
        spawnDuration = 3f;
        spawnMinDuration = 2f;
        normalSpawnProbability = 10;
        onWorkSpawnProbability = 40;
        lunchSpawnProbability = 30;
        offWorkSpawnProbability = 50;
        for (int idx = 0; idx < maxFloorNum; idx++)
        {
            Floor floor = Instantiate(floorPrefab);
            floor.name += $"{idx}";
            floor.Init(slotPrefab, idx, FloorName.NameList[idx], FloorTimerPrefab, daySpawnDuration, spawnMinDuration,
                spawnMinDuration);
            Floors.Add(floor.GetComponent<Floor>());
            if (floor.FloorIdx < initialFloorNum)
            {
                floor.Actiavte();
            }
        }
    }

    public void SetIsPlaying(bool b)
    {
        foreach (var floor in Floors)
        {
            floor.isPlaying = b;
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
        List<Passenger> passengersToOnboard =
            e.ArrivalFloor.GetPassengersToOnboard(e.Elevator, e.RemainWeight, e.AfterDirection);
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


    public List<int> usingChangeList = new List<int>();
    public List<float> originalValues = new List<float>();

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

                SetIsPlaying(true);
                break;
            case DayEventType.DayEnded:
                SetIsPlaying(false);
                break;
            case DayEventType.WaveStarted:
                WaveType waveType = (WaveType)e.Args;
                int currentDay = DayManager.Instance.Day;
                if (waveType == WaveType.Lunch)
                {
                    for (int i = 0; i < lunchActiveNumber[currentDay]; i++)
                    {
                        int idx = Random.Range(0, GetNowFloorNum());
                        usingChangeList.Add(idx);
                        if (Floors[idx].DaySpawnDuration.Count > currentDay)
                        {
                            originalValues.Add(Floors[idx].DaySpawnDuration[currentDay]);
                            Floors[idx].DaySpawnDuration[currentDay] *= lunchMultiplier;
                        }
                    }
                }

                this.ApplyWave((WaveType)e.Args);
                break;
            case DayEventType.WaveEnded:
                WaveType waveType2 = (WaveType)e.Args;
                if (waveType2 == WaveType.Lunch)
                {
                    foreach (var idx in usingChangeList)
                    {
                        int currentDayResume = DayManager.Instance.Day;
                        if (Floors[idx].DaySpawnDuration.Count > currentDayResume)
                        {
                            Floors[idx].DaySpawnDuration[currentDayResume] = originalValues[currentDayResume];
                        }
                    }

                    usingChangeList.Clear();
                    originalValues.Clear();
                }
                
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

    public int GetNormalProbability()
    {
        return normalSpawnProbability + DayManager.Instance.Day;
    }

    public int GetOnWorkProbability()
    {
        return onWorkSpawnProbability + DayManager.Instance.Day;
    }

    public int GetLunchProbability()
    {
        return lunchSpawnProbability + DayManager.Instance.Day / 2;
    }

    public int GetOffWorkProbabiltiy()
    {
        return offWorkSpawnProbability + DayManager.Instance.Day;
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