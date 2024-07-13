using System;
using System.Collections;
using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoldManager : Singleton<GoldManager>, EventListener<ElevatorSettleUpEvent>
{
    public int CurrentGold = 5000;

    public float GoldPerSpeed = 1;
    public GameObject IncomeTextPrefab;

    public void OnEvent(ElevatorSettleUpEvent e)
    {
        int unitPriceBySpeed = GetUnitPriceBySpeed(e.Elevator._speed);
        float weight = e.AffectedCapacity + GetEmptyWeight(e.Elevator._maxCapacity);
        int minusAffectedGold =
            (int)(unitPriceBySpeed * weight * Mathf.Abs(e.ArrivalFloor.FloorIdx - e.StartFloor.FloorIdx));
        int plustAffectedGold = 0;
        for (int i = 0; i < e.ExitWantCount; i++)
        {
            plustAffectedGold += Random.Range(50, 200);
        }

        int beforeGold = CurrentGold;
        CurrentGold += plustAffectedGold;
        CurrentGold -= minusAffectedGold;
        if (plustAffectedGold > 0)
        {
            GameObject plusObj = Instantiate(IncomeTextPrefab);
            plusObj.transform.position =
                new Vector3(e.Elevator.transform.position.x, e.Elevator.transform.position.y + 0.2f, -9);
            plusObj.GetComponent<IncomeScript>().Message = "+" + plustAffectedGold;
        }

        GoldChangedEvent.Trigger(
            plustAffectedGold,
            minusAffectedGold,
            beforeGold,
            CurrentGold
        );

        // float a = GetEmptyElevator
        // throw new System.NotImplementedException();
    }

    private float GetEmptyWeight(int elevatorMaxCapacity)
    {
        if (elevatorMaxCapacity >= 8)
        {
            return 2;
        }

        if (elevatorMaxCapacity >= 6)
        {
            return 1.5f;
        }

        return 1;
    }

    public int GetUnitPriceBySpeed(float speed)
    {
        if (speed >= 8.99f)
        {
            return 20;
        }

        if (speed >= 5.99f)
        {
            return 15;
        }

        return 10;
    }

    void OnEnable()
    {
        this.StartListeningEvent();
    }

    void OnDisable()
    {
        this.StopListeningEvent();
    }
}