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
        
        UpgradeEvent.Trigger(UpgradeEventType.Updated);
    }

    public bool CanUpgradeSpace(int idx)
    {
        return ElevatorUpgrades[idx].CurrentSpaceLevel + 1 < SpaceUpgrades.Count;
    }

    public void UpgradeSpeed(int idx)
    {
        if (!CanUpgradeSpeed(idx))
            return;
        ElevatorUpgrades[idx].CurrentSpeedLevel += 1;
        
        ElevatorManager.Instance.SetSpeed(idx,ElevatorUpgrades[idx].CurrentSpeedUpgrade.AdditionalMaxSpeed);
        
        UpgradeEvent.Trigger(UpgradeEventType.Updated);
    }

    public bool CanUpgradeSpeed(int idx)
    {
        return ElevatorUpgrades[idx].CurrentSpeedLevel + 1 < SpeedUpgrades.Count;
    }

    public void UpgradeOptCostLevel(int idx)
    {
        if (!CanUpgradeOptCostLevel(idx))
            return;
        ElevatorUpgrades[idx].CurrentOptCostLevel += 1;
        
        UpgradeEvent.Trigger(UpgradeEventType.Updated);
    }

    public bool CanUpgradeOptCostLevel(int idx)
    {
        return ElevatorUpgrades[idx].CurrentOptCostLevel + 1 < OptCostUpgrades.Count;
    }
    public List<UpgradeData> GetElevatorNextUpgrades(int idx)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        ElevatorUpgradeStatus elevatorUpgradeStatus = ElevatorUpgrades[idx];

        if (CanUpgradeSpace(idx))
        {
            result.Add(this.SpaceUpgrades[elevatorUpgradeStatus.CurrentSpaceLevel + 1]);
        }

        if (CanUpgradeSpeed(idx))
        {
            result.Add(this.SpeedUpgrades[elevatorUpgradeStatus.CurrentSpeedLevel + 1]);
        }

        if (CanUpgradeOptCostLevel(idx))
        {
            result.Add(this.OptCostUpgrades[elevatorUpgradeStatus.CurrentOptCostLevel + 1]);
        }

        return result;
    }
}