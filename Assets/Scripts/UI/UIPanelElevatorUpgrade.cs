using UnityEngine;
using UnityEngine.UI;

public class UIPanelElevatorUpgrade : UIPanel
{
    [Header("Binding")]
    public Button CloseButton;

    protected override void Awake()
    {
        base.Awake();
        this.CloseButton.onClick.AddListener(this.OnClickClose);
    }

    public override void Show(Transform character)
    {
        // data setting
        base.Show(character);
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
    
}