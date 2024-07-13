using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private int FloorIdx;
    private int SlotIdx;
    
    public void Init(int floorIdx, int slotIdx)
    {
        FloorIdx = floorIdx;
        SlotIdx = slotIdx;
    }

    private void Start()
    {
        SlotUiData uiData = SlotUiData.GetSlotUiData(FloorIdx, SlotIdx);
        transform.localPosition = new Vector3(uiData.localPosX, uiData.localPosY, 0);
    }
}
