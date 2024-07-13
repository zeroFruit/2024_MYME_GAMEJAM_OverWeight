using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int FloorIdx;

    [Header("FloorInfo")] public int capacityOfPassengers;
    public int maxCapacityOfPassengers;
    [Header("SpawnProbability")] public int normalSpawnProbability;
    public int onWorkSpawnProbability;
    public int lunchSpawnProbability;
    public int offWorkSpawnProbability;
    [Header("InnerData")] public List<Slot> Slots;
    public List<Passenger> Passengers;
    public FloorTimer timer;

    public WaveType WaveType;


    public void Init(Slot slotPrefab, int floorIdx, FloorTimer floorTimer)
    {
        FloorIdx = floorIdx;
        capacityOfPassengers = 7;
        maxCapacityOfPassengers = 10;

        normalSpawnProbability = 20;
        onWorkSpawnProbability = 70;
        lunchSpawnProbability = 40;
        offWorkSpawnProbability = 70;

        Slots = new List<Slot>();
        for (int idx = 0; idx < maxCapacityOfPassengers; idx++)
        {
            Slot slot = Instantiate(slotPrefab, transform);
            slot.Init(floorIdx, idx);
            Slots.Add(slot);
        }

        Passengers = new List<Passenger>();
        timer = Instantiate(floorTimer, transform);
        timer.Init();
        WaveType = WaveType.None;
    }

    public void Start()
    {
        FloorUiData uiData = FloorUiData.GetFloorUiData(FloorIdx);
        transform.localPosition = new Vector3(uiData.localPosX, uiData.localPosY, 0);
    }

    public void SpawnPassenger()
    {
        // 승객 초과 타이머 체크
        if (Passengers.Count >= capacityOfPassengers)
        {
            timer.Progress(10f);
        }

        // 최대 승객 수까지 승객 생성
        if (Passengers.Count < maxCapacityOfPassengers)
        {
            Debug.Log("WaveType : " + WaveType);
            // 웨이브에 맞게 승객 생성
            switch (this.WaveType)
            {
                case WaveType.None:
                    SpawnNoneWave();
                    break;
                case WaveType.OnWork:
                    SpawnOnWorkWave();
                    break;
                case WaveType.Lunch:
                    SpawnLunchWave();
                    break;
                case WaveType.OffWork:
                    SpawnOffWorkWave();
                    break;
                default:
                    Debug.Log("Floor: Unhandled WaveType! - " + WaveType.ToString());
                    break;
            }
        }
    }

    private void SpawnNoneWave()
    {
        // 스폰 여부 체크
        if (GetResultFromProbability(normalSpawnProbability))
        {
            spawn();
        }
    }

    private void SpawnOnWorkWave()
    {
        // 1층이 아닌 층은 일반 스폰
        if (this.FloorIdx != 0)
        {
            SpawnNoneWave();
            return;
        }

        // 1층은 출근 확률 적용 스폰
        if (GetResultFromProbability(onWorkSpawnProbability))
        {
            spawn();
        }
    }

    private void SpawnLunchWave()
    {
        // 점심먹으러 1층 가는 사람 스폰
        if (GetResultFromProbability(lunchSpawnProbability))
        {
            spawn(FloorManager.Instance.Floors.First());
            return;
        }

        // 점심먹고 올라오는 사람 스폰
        if (FloorIdx == 0)
        {
            if (GetResultFromProbability(lunchSpawnProbability))
            {
                spawn();
                return;
            }
        }

        // 나머지
        if (GetResultFromProbability(normalSpawnProbability))
        {
            spawn();
        }
    }

    private void SpawnOffWorkWave()
    {
        // 스폰 여부 체크
        if (GetResultFromProbability(normalSpawnProbability))
        {
            // 퇴근길로 1층 갈 확률 적용
            if (GetResultFromProbability(offWorkSpawnProbability))
            {
                spawn(FloorManager.Instance.Floors.First());
                return;
            }

            // 퇴근이 아닌 승객 스폰
            spawn();
        }
    }

    private void spawn(Floor target = null)
    {
        Passenger newPassenger = PassengerManager.Instance.Spawn(this, target);
        newPassenger.transform.SetParent(getEmptySlot().transform);
        Passengers.Add(newPassenger);
    }

    private Slot getEmptySlot()
    {
        return Slots[Passengers.Count];
    }

    private bool GetResultFromProbability(int probability)
    {
        int rand = Random.Range(0, 100);
        return rand <= probability;
    }

    public List<Passenger> GetPassengersToOnboard(ElevatorController elevator, int remainWeight,
        ElevatorDirection afterDirection)
    {
        int usedWeight = 0;
        List<Passenger> passengersToOnboard = new List<Passenger>();
        foreach (var passenger in Passengers)
        {
            ElevatorDirection passengerDirection = passenger.StartFloor.DirectionTo(passenger.TargetFloor);
            if (!elevator.IsStoppable(passenger.StartFloor, passenger.TargetFloor))
            {
                continue;
            }
            if (afterDirection != ElevatorDirection.UNWARE && passengerDirection != afterDirection)
            {
                continue;
            }

            if (usedWeight + passenger.Weight <= remainWeight)
            {
                usedWeight += passenger.Weight;
                afterDirection = passengerDirection; // 첫번째 사람으로 direction 고정
                passengersToOnboard.Add(passenger);
            }
        }

        return passengersToOnboard;
    }

    public void OnboardPassenger(Passenger passenger)
    {
        Passengers.Remove(passenger);
        RearrangePassengers();
        if (Passengers.Count() < capacityOfPassengers)
        {
            timer.ResetProgress();
        }
    }

    private void RearrangePassengers()
    {
        foreach (var slot in Slots)
        {
            Passenger child = slot.GetComponentInChildren<Passenger>();
            if (child != null)
            {
                slot.GetComponentInChildren<Passenger>().transform.SetParent(null);
            }
        }

        for (int idx = 0; idx < Passengers.Count; idx++)
        {
            Passengers[idx].transform.SetParent(Slots[idx].transform);
        }
    }

    public ElevatorDirection DirectionTo(Floor to)
    {
        if (FloorIdx > to.FloorIdx)
        {
            return ElevatorDirection.Down;
        }

        if (FloorIdx < to.FloorIdx)

        {
            return ElevatorDirection.Up;
        }

        return ElevatorDirection.UNWARE;
    }


    public static bool operator >(Floor left, Floor right)
    {
        return left.FloorIdx > right.FloorIdx;
    }

    public static bool operator <(Floor left, Floor right)
    {
        return left.FloorIdx < right.FloorIdx;
    }

    public void ResetFloor()
    {
        timer.ResetProgress();
        Passengers.Clear();
        RearrangePassengers();
    }

    public void changeWave(WaveType waveType)
    {
        this.WaveType = waveType;
    }
}