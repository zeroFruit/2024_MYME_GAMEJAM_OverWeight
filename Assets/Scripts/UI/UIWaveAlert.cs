using HAWStudio.Common;
using UnityEngine;

public class UIWaveAlert : UIPanel
{
    public FeedbackPlayer AlertFeedback;

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
}