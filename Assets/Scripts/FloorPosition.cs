public struct FloorUiData
{
    public float localPosX;
    public float localPosY;

    FloorUiData(int floorIdx)
    {
        localPosX = 7.4f;
        localPosY = -14.7f + floorIdx * 2f;
    }

    public static FloorUiData GetFloorUiData(int floorIdx)
    {
        return new FloorUiData(floorIdx);
    }
}