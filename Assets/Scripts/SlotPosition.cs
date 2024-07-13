public struct SlotUiData
{
    public float localPosX;
    public float localPosY;

    SlotUiData(int floorIdx, int slotIdx)
    {
        localPosX = 0f - (2f * slotIdx);
        localPosY = 0f;
    }

    public static SlotUiData GetSlotUiData(int floorIdx, int slotIdx)
    {
        return new SlotUiData(floorIdx, slotIdx);
    }
}