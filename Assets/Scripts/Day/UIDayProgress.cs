using System;
using SSR.OverWeight;
using UnityEngine;

public class DayProgress
{
    public float Current;
    public float Max;
}

public class UIDayProgress : MonoBehaviour,
    EventListener<DayEvent>
{
    UIProgressBar _progressBar;

    void Awake()
    {
        this._progressBar = this.GetComponentInChildren<UIProgressBar>();
        this._progressBar.UpdateBar(0f, 0f, DayManager.Instance.DayLengthSeconds);
    }

    void OnProgressUpdated(float current, float max)
    {
        this._progressBar.UpdateBar(current, 0f, DayManager.Instance.DayLengthSeconds);
    }

    public void OnEvent(DayEvent e)
    {
        switch (e.EventType)
        {
            case DayEventType.ProgressUpdated:
                DayProgress dayProgress = (DayProgress)e.Args;
                this.OnProgressUpdated(dayProgress.Current, dayProgress.Max);
                break;
        }
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