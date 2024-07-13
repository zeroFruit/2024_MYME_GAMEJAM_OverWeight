using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISectionElevatorUpgrade : MonoBehaviour
{
    public int Idx;
    public TextMeshProUGUI Title;
    List<UIItem> _upgradeSections;

    void Awake()
    {
        this.Title.text = $"엘레베이터 {this.Idx + 1}";
        this._upgradeSections = new List<UIItem>(this.GetComponentsInChildren<UIItem>());
        foreach (UIItem item in this._upgradeSections)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        
    }
    
    void ActivateSections()
    {
        for (int i = 0; i < UpgradeManager.Instance.ElevatorUpgrades.Count; i++)
        {
            this._upgradeSections[i].gameObject.SetActive(true);
            this._upgradeSections[i].Initialize();
        }
    }
    
    void DeactivateSections()
    {
        for (int i = 0; i < _upgradeSections.Count; i++)
        {
            this._upgradeSections[i].gameObject.SetActive(false);
        }
    }
}