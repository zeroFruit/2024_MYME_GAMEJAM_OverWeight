using System;
using UnityEngine;
using UnityEngine.UI;

public class UISectionElevatorStatus : MonoBehaviour
{
    [Header("Binding")]
    public Button ChangeAlgorithmButton;
    public UIPanel AlgorithmSelectionPanel;

    void Awake()
    {
        this.ChangeAlgorithmButton.onClick.AddListener(this.OnClickChangeAlgorithm);
    }

    void OnClickChangeAlgorithm()
    {
        this.AlgorithmSelectionPanel.Show(null);
    }
}