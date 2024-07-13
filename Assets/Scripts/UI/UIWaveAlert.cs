using System;
using System.Collections.Generic;
using HAWStudio.Common;
using SSR.OverWeight;
using TMPro;
using UnityEngine;

public class UIWaveAlert : UIPanel,
    EventListener<DayEvent>
{
    public TextMeshProUGUI WaveNameText;
    public FeedbackPlayer AlertFeedback;

    Dictionary<WaveType, string> _texts = new Dictionary<WaveType, string>
    {
        { WaveType.OnWork, "출근 시간" },
        { WaveType.Lunch, "점심 시간" },
        { WaveType.OffWork, "퇴근 시간" },
    };

    public void Show(WaveType _currentWave)
    {
        this.WaveNameText.text = this._texts[_currentWave];
        this.Show(null);
    }
    
    public override void Show(Transform character)
    {
        base.Show(character);
        this.AlertFeedback.PlayFeedbacks();
    }

    #region Debugs

    [InspectorButton("TestShow")]
    public bool TestShowButton;
    [InspectorButton("TestHide")]
    public bool TestHideButton;
    
    void TestShow()
    {
        this.AlertFeedback.PlayFeedbacks();    
    }
    
    void TestHide()
    {
        this.Hide();
    }

    #endregion

    public void OnEvent(DayEvent e)
    {
        switch (e.EventType)
        {
            case DayEventType.WaveStarted:
                this.Show((WaveType)e.Args);
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