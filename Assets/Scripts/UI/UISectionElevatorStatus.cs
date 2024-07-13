using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISectionElevatorStatus : MonoBehaviour
{
    [Header("Binding")]
    public int ElevatorIndex;

    public void SetElevatorIndex(int index)
    {
        ElevatorIndex = index;
    }

    void Update() {

    }
}