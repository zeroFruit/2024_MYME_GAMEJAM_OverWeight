using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelElevatorUpgrade : UIPanel
{
    #region Binding

    public enum GameObjects
    {
        ItemContent,
    }

    GameObject _itemContentRoot;

    #endregion
    
    [Header("Binding")]
    public Button CloseButton;

    List<UIItem> _items = new List<UIItem>();

    List<UISectionElevatorUpgrade> _upgradeSections;

    protected override void Awake()
    {
        base.Awake();
        this.CloseButton.onClick.AddListener(this.OnClickClose);
        
        this._upgradeSections =
            new List<UISectionElevatorUpgrade>(this.GetComponentsInChildren<UISectionElevatorUpgrade>());
        foreach (UISectionElevatorUpgrade section in this._upgradeSections)
        {
            section.gameObject.SetActive(false);
        }
        
        this.BindUI();
    }

    void BindUI()
    {
        Bind<GameObject>(typeof(GameObjects));

        this._itemContentRoot = Get<GameObject>((int)GameObjects.ItemContent);
    }

    public override void Show(Transform character)
    {
        this.ActivateSections();
        
        base.Show(character);
    }

    public override void Hide()
    {
        base.Hide();
        
        this.DeactivateSections();
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