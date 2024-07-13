using System.Collections.Generic;
using SSR.OverWeight;
using TMPro;
using UnityEngine;

public class UISectionElevatorUpgrade : MonoBehaviour,
    EventListener<UpgradeEvent>,
    EventListener<GoldChangedEvent>
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
        this.DeactivateSections();
        this.ActivateSections();
    }
    
    void ActivateSections()
    {
        List<UpgradeData> nextUpgrades = UpgradeManager.Instance.GetElevatorNextUpgrades(this.Idx);
        for (int i = 0; i < nextUpgrades.Count; i++)
        {
            this._upgradeSections[i].gameObject.SetActive(true);
            this._upgradeSections[i].Initialize(this.Idx, nextUpgrades[i]);
        }
    }
    
    void DeactivateSections()
    {
        for (int i = 0; i < _upgradeSections.Count; i++)
        {
            this._upgradeSections[i].gameObject.SetActive(false);
        }
    }
    
    public void OnEvent(UpgradeEvent e)
    {
        switch (e.EventType)
        {
            case UpgradeEventType.Updated:
                this.Initialize();
                break;
        }
    }
    
    public void OnEvent(GoldChangedEvent e)
    {
        
        this.Initialize();   
    }

    void OnEnable()
    {
        this.StartListeningEvent<UpgradeEvent>();
        this.StartListeningEvent<GoldChangedEvent>();
    }

    void OnDisable()
    {
        this.StopListeningEvent<UpgradeEvent>();
        this.StopListeningEvent<GoldChangedEvent>();
    }
}