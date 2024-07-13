using UnityEngine;
using UnityEngine.UI;

public class UIPanelElevatorAlgorithm : UIPanel
{
    [Header("Binding")]
    public Button CloseButton;

    protected override void Awake()
    {
        base.Awake();
        this.CloseButton.onClick.AddListener(this.OnClickClose);
    }
    
    void OnClickClose()
    {
        this.Hide();
    }
}