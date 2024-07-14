using System;
using System.Collections;
using System.Collections.Generic;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.Serialization;

public enum DayState
{
    OnGoing,
    Ended,
}

public class DayManager : Singleton<DayManager>
{
    public float DayLengthSeconds = 60f;
    public List<DayWave> DayWaves;
    public int Day;
    public bool isInProgress;
    
    public float Progress;

    float _progressTimer = 0f;
    float _progressUpdateInterval = 1f;

    Dictionary<WaveType, bool> _dayWavesCompleted = new Dictionary<WaveType, bool>();

    WaveType _currentWave;

    protected override void Awake()
    {
        base.Awake();
        this.Initialize();
        StartCoroutine(sendOnStart());
    }

    private IEnumerator sendOnStart()
    {
        yield return null;
        StartDay();
    }
    
    public void Initialize()
    {
        this.Progress = 0f;
        this._currentWave = WaveType.None;

        foreach (DayWave dayWave in this.DayWaves)
        {
            this._dayWavesCompleted[dayWave.WaveType] = false;
        }

        this.Day = 0;
        this.isInProgress = false;
    }

    private void StartDay()
    {
        this.Progress = 0f;
        this._progressTimer = 0f;
        this.isInProgress = true;
        DayEvent.Trigger(DayEventType.DayStarted);
    }

    private void StopDay()
    {
        this.isInProgress = false;
        DayEvent.Trigger(DayEventType.DayEnded);
    }
    
    void Update()
    {
        this.ProgressDay();
    }

    void ProgressDay()
    {
        if (!isInProgress)
        {
            return;
        }
        
        this._progressTimer += Time.deltaTime;
        if (this._progressTimer < this._progressUpdateInterval)
        {
            return;
        }

        this._progressTimer = 0f;

        this.Progress += (int)this._progressUpdateInterval;

        if (this.Progress >= this.DayLengthSeconds)
        {
            StopDay();
            Day++;
            StartDay();
            return;
        }

        this.CheckWave();
        
        DayEvent.Trigger(DayEventType.ProgressUpdated,new DayProgress
        {
            Current = this.Progress,
            Max = this.DayLengthSeconds,
        });
    }

    void CheckWave()
    {
        foreach (DayWave dayWave in this.DayWaves)
        {
            if (this.Progress > dayWave.StartTime && this.Progress < dayWave.EndTime)
            {
                if (this._currentWave == WaveType.None)
                {
                    // 웨이브 시작
                    this._currentWave = dayWave.WaveType;
                    DayEvent.Trigger(DayEventType.WaveStarted, this._currentWave);
                    // Debug.Log($"wave started: {this._currentWave}");
                    return;
                }

                if (dayWave.WaveType == this._currentWave)
                {
                    // 현재 웨이브 진행중
                    return;
                }
            }
            
            if (dayWave.WaveType == this._currentWave)
            {
                // 웨이브 끝
                // Debug.Log($"wave ended: {this._currentWave}");
                this._currentWave = WaveType.None;
                DayEvent.Trigger(DayEventType.WaveEnded, this._currentWave);
                return;
            }
            
            // 다음 웨이브 체크
        }
    }
}