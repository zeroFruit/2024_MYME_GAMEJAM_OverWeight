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
    public List<UpgradeData> AlgorithmUpgrades;
    public List<UpgradeData> SpaceUpgrades;
    public List<UpgradeData> SpeedUpgrades;
    public List<UpgradeData> OptCostUpgrades;

    [Header("State")]
    public List<string> CurrentHavingAlgorithms; 
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

    public List<UpgradeData> GetUpgradeableAlgorithm()
    {
        List<UpgradeData> result = new List<UpgradeData>();

        foreach (KeyValuePair<string,UpgradeData> keyValuePair in this._upgradeDictionary)
        {
            if (!this.CurrentHavingAlgorithms.Contains(keyValuePair.Key))
            {
                result.Add(keyValuePair.Value);
            }
        }

        return result;
    }
}