using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelElevatorSetting : UIPanel
{
    [Header("Binding")]
    public Button CloseButton;
    public List<UISectionElevatorStatus> elevatorSettingUIs = new List<UISectionElevatorStatus>();

    protected override void Awake()
    {
        base.Awake();
        this.CloseButton.onClick.AddListener(this.OnClickClose);

        if (elevatorSettingUIs.Count != 3)
        {
            Debug.LogError($"UIPanelElevatorSetting: elevatorSettingUIs is not three {elevatorSettingUIs.Count}");
        }
    }

    void OnClickClose()
    {
        this.Hide();
    }

    #region Debugs

    [InspectorButton("TestShow")]
    public bool TestShowButton;
    [InspectorButton("TestHide")]
    public bool TestHideButton;
    
    void TestShow()
    {
        this.Show(null);
    }
    
    void TestHide()
    {
        this.Hide();
    }

    #endregion
    

    void Update() {
        var elevators = ElevatorManager.Instance._elevators;
        for (int i = 0; i < 3; i++)
        {
            if (i >= elevators.Count)
            {
                elevatorSettingUIs[i].gameObject.SetActive(false);
                continue;
            }
            elevatorSettingUIs[i].gameObject.SetActive(true);
        }
    }
    
}