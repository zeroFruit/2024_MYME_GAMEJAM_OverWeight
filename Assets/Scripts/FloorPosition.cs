public struct FloorUiData
{
    public float localPosX;
    public float localPosY;
    public float scaleX;
    public float scaleY;

    FloorUiData(int floorIdx)
    {
        localPosX = -8.47f;
        localPosY = -14.06f + floorIdx * 2f;
        scaleX = 5.5f;
        scaleY = 0.36f;
    }

    public static FloorUiData GetFloorUiData(int floorIdx)
    {
        return new FloorUiData(floorIdx);
    }
}