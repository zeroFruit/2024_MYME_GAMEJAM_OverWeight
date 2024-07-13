using UnityEngine;
using UnityEngine.UI;

public class HudElevatorControls : MonoBehaviour
{
    [Header("Bindings")]
    public Button UpgradeButton;
    public Button SettingsButton;

    public UIPanel UpgradePanel;
    public UIPanel SettingsPanel;

    void Awake()
    {
        this.UpgradeButton.onClick.AddListener(this.OnClickUpgrade);
        this.SettingsButton.onClick.AddListener(this.OnClickSetting);
    }

    void OnClickUpgrade()
    {
        Debug.Log("Clicked Upgrade");
        this.UpgradePanel.Show(null);
    }

    void OnClickSetting()
    {
        Debug.Log("Clicked Setting");
        this.SettingsPanel.Show(null);
    }
}