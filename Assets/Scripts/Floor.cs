using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    [Header("FloorInfo")] 
    public int FloorIdx;
    public string FloorName;

    public int capacityOfPassengers;
    public int maxCapacityOfPassengers;

    [Header("InnerData")] public List<Slot> Slots;
    public List<Passenger> Passengers;
    public FloorTimer timer;
    public bool isActivated;

    public WaveType WaveType;


    public void Init(Slot slotPrefab, int floorIdx, string floorName, FloorTimer floorTimer)
    {
        FloorIdx = floorIdx;
        FloorName = floorName;
        capacityOfPassengers = 7;
        maxCapacityOfPassengers = 10;

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
        this.isActivated = false;
        WaveType = WaveType.None;

        TextMeshPro textField = this.GetComponentInChildren<TextMeshPro>();
        textField.transform.localPosition = new Vector3(-23f, 0.7f, 0);
        textField.text = GetFloorName();
        textField.fontSize = 8f;
        textField.fontStyle = FontStyles.Bold;
        textField.rectTransform.sizeDelta = new Vector2(14, 1);
    }

    public void Start()
    {
        FloorUiData uiData = FloorUiData.GetFloorUiData(FloorIdx);
        transform.localPosition = new Vector3(uiData.localPosX, uiData.localPosY, 0);
    }

    private void Update()
    {
        TextMeshPro textField = this.GetComponentInChildren<TextMeshPro>();
        textField.text = GetFloorName();
        Color color = isActivated ? new Color(0, 0, 0) : new Color(0.5f, 0.5f, 0.5f);
        textField.color = color;
        textField.outlineColor = color;
    }

    private string GetFloorName()
    {
        return $"{FloorIdx + 1}F {FloorName} [{Passengers.Count}/{maxCapacityOfPassengers}]";
    }

    public void Actiavte()
    {
        this.isActivated = true;
    }

    public void SpawnPassenger()
    {
        if (!isActivated)
        {
            return;
        }
        
        // 승객 초과 타이머 체크
        if (Passengers.Count >= capacityOfPassengers)
        {
            timer.Progress(10f);
            foreach (Passenger passenger in this.Passengers)
            {
                passenger.Angry();
            }
        }
        else
        {
            foreach (Passenger passenger in this.Passengers)
            {
                passenger.CalmDown();
            }
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
        if (GetResultFromProbability(FloorManager.Instance.GetNormalProbability()))
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
        if (GetResultFromProbability(FloorManager.Instance.GetOnWorkProbability()))
        {
            spawn();
        }
    }

    private void SpawnLunchWave()
    {
        // 점심먹으러 1층 가는 사람 스폰
        if (this.FloorIdx != 0 && GetResultFromProbability(FloorManager.Instance.GetLunchProbability()))
        {
            spawn(FloorManager.Instance.Floors.First());
            return;
        }

        // 점심먹고 올라오는 사람 스폰
        if (FloorIdx == 0)
        {
            if (GetResultFromProbability(FloorManager.Instance.GetLunchProbability()))
            {
                spawn();
                return;
            }
        }

        // 나머지
        if (GetResultFromProbability(FloorManager.Instance.GetNormalProbability()))
        {
            spawn();
        }
    }

    private void SpawnOffWorkWave()
    {
        // 스폰 여부 체크
        if (GetResultFromProbability(FloorManager.Instance.GetNormalProbability()))
        {
            // 퇴근길로 1층 갈 확률 적용
            if (this.FloorIdx != 0 && GetResultFromProbability(FloorManager.Instance.GetOffWorkProbabiltiy()))
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
        passenger.transform.SetParent(null);
        Passengers.Remove(passenger);
        RearrangePassengers();
        if (Passengers.Count() < capacityOfPassengers)
        {
            timer.ResetProgress();
        }
    }

    private void RearrangePassengers()
    {
        List<Passenger> passengersToMove = new List<Passenger>();
        foreach (var slot in Slots)
        {
            Passenger child = slot.GetComponentInChildren<Passenger>();
            if (child != null)
            {
                Passenger p = slot.GetComponentInChildren<Passenger>();
                p.transform.SetParent(null);
                passengersToMove.Add(p);
            }
        }

        for (int idx = 0; idx < passengersToMove.Count; idx++)
        {
            passengersToMove[idx].transform.SetParent(Slots[idx].transform);
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

    public void StopFloor()
    {
        timer.StopProgress();
        RearrangePassengers();
    }

    public void changeWave(WaveType waveType)
    {
        this.WaveType = waveType;
    }
}