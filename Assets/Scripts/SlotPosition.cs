public struct SlotUiData
{
    public float localPosX;
    public float localPosY;
    public float scaleX;
    public float scaleY;

    SlotUiData(int floorIdx, int slotIdx)
    {
        scaleX = 0.3f;
        scaleY = 6f;
        localPosX = 2.86f - scaleX*slotIdx;
        localPosY = -2.74f;
    }

    public static SlotUiData GetSlotUiData(int floorIdx, int slotIdx)
    {
        return new SlotUiData(floorIdx, slotIdx);
    }
}