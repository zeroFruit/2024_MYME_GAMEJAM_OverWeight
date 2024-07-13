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

    protected override void Awake()
    {
        base.Awake();
        this.CloseButton.onClick.AddListener(this.OnClickClose);
        this.BindUI();
    }

    void BindUI()
    {
        Bind<GameObject>(typeof(GameObjects));

        this._itemContentRoot = Get<GameObject>((int)GameObjects.ItemContent);
    }

    public override void Show(Transform character)
    {
        this.ClearAllItems();
        // this.InstantiateItems();
        
        base.Show(character);
    }

    void ClearAllItems()
    {
        foreach (UIItem uiItem in this._items)
        {
            Destroy(uiItem.gameObject);
        }

        this._items = new List<UIItem>();
    }

    void InstantiateItems(List<UpgradeData> upgradeDataList)
    {
        
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