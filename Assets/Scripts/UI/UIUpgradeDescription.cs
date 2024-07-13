using TMPro;
using UnityEngine;

public class UIUpgradeDescription : MonoBehaviour
{
    public int elevatorIndex;
    public TMP_Text upgradeDescriptionText;
    public void SetElevatorIndex(int index)
    {
        elevatorIndex = index;
    }

    public void Update() {
        var elevatorManager = ElevatorManager.Instance;
        if (elevatorManager._elevators.Count <= elevatorIndex) {
            return;
        }
        var upgrade = UpgradeManager.Instance.ElevatorUpgrades[elevatorIndex];

        upgradeDescriptionText.text = $"공간 업그레이드 Lv. {upgrade.CurrentSpaceLevel}\n속도 업그레이드 Lv. {upgrade.CurrentSpeedLevel}\n비용 절감 업그레이드 Lv. {upgrade.CurrentOptCostLevel}";
    }
}
