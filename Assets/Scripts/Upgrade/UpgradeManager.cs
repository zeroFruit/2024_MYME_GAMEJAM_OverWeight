using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElevatorUpgradeStatus
{
    public int CurrentSpaceLevel;
    public int CurrentSpeedLevel;
    public int CurrentOptCostLevel;

    public UpgradeData CurrentSpaceUpgrade => UpgradeManager.Instance.SpaceUpgrades[CurrentSpaceLevel];
    public UpgradeData CurrentSpeedUpgrade => UpgradeManager.Instance.SpeedUpgrades[CurrentSpeedLevel];
    public UpgradeData CurrentOptCostUpgrade => UpgradeManager.Instance.OptCostUpgrades[CurrentOptCostLevel];
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    [Header("Data")]
    public List<UpgradeData> SpaceUpgrades;
    public List<UpgradeData> SpeedUpgrades;
    public List<UpgradeData> OptCostUpgrades;

    [Header("State")]
    public List<ElevatorUpgradeStatus> ElevatorUpgrades;

    Dictionary<string, UpgradeData> _upgradeDictionary = new Dictionary<string, UpgradeData>();

    protected override void Awake()
    {
        base.Awake();
        this.Initialize();
    }

    void Initialize()
    {
        foreach (UpgradeData upgradeData in Resources.LoadAll<UpgradeData>("Upgrade"))
        {
            this._upgradeDictionary[upgradeData.name] = upgradeData;
        }
    }

    /// <summary>
    /// 엘레베이터 추가할 때 호출해야함
    /// </summary>
    public void AddElevator()
    {
        this.ElevatorUpgrades.Add(new ElevatorUpgradeStatus());
    }

    public void UpgradeSpace(int idx)
    {
        if (!CanUpgradeSpace(idx))
            return;
        ElevatorUpgrades[idx].CurrentSpaceLevel += 1;
        ElevatorManager.Instance.SetElevatorCapacity(idx,ElevatorUpgrades[idx].CurrentSpaceUpgrade.MaxCapacity);
        ElevatorManager.Instance.SetEmptyWeight(idx,ElevatorUpgrades[idx].CurrentSpaceUpgrade.EmptyWeight);
    }

    public bool CanUpgradeSpace(int idx)
    {
        return ElevatorUpgrades[idx].CurrentSpaceLevel < SpaceUpgrades.Count;
    }

    public void UpgradeSpeed(int idx)
    {
        if (!CanUpgradeSpeed(idx))
            return;
        ElevatorUpgrades[idx].CurrentSpeedLevel += 1;
    }

    public bool CanUpgradeSpeed(int idx)
    {
        return ElevatorUpgrades[idx].CurrentSpeedLevel < SpeedUpgrades.Count;
    }

    public void UpgradeOptCostLevel(int idx)
    {
        if (!CanUpgradeOptCostLevel(idx))
            return;
        ElevatorUpgrades[idx].CurrentOptCostLevel += 1;
    }

    public bool CanUpgradeOptCostLevel(int idx)
    {
        return ElevatorUpgrades[idx].CurrentOptCostLevel < OptCostUpgrades.Count;
    }
    public List<UpgradeData> GetElevatorUpgrades(int idx)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        ElevatorUpgradeStatus elevatorUpgradeStatus = ElevatorUpgrades[idx];

        if (elevatorUpgradeStatus.CurrentSpaceLevel < this.SpaceUpgrades.Count)
        {
            result.Add(this.SpaceUpgrades[elevatorUpgradeStatus.CurrentSpaceLevel]);
        }

        if (elevatorUpgradeStatus.CurrentSpeedLevel < this.SpeedUpgrades.Count)
        {
            result.Add(this.SpeedUpgrades[elevatorUpgradeStatus.CurrentSpeedLevel]);
        }

        if (elevatorUpgradeStatus.CurrentOptCostLevel < this.OptCostUpgrades.Count)
        {
            result.Add(this.OptCostUpgrades[elevatorUpgradeStatus.CurrentOptCostLevel]);
        }

        return result;
    }
}