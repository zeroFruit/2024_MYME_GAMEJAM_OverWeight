using System;

public enum WaveType
{
    None,
    OnWork,
    Lunch,
    OffWork,
}

[Serializable]
public class DayWave
{
    public WaveType WaveType;
    public float StartTime;
    public float EndTime;
}