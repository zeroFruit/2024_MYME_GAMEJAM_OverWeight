using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : UIBase
{
    public enum Texts
    {
        Text_Name,
        Text_Description,
        Text_Button,
        Text_Level,
    }

    public enum Buttons
    {
        Button_Upgrade
    }

    public enum Images
    {
        Image_Upgrade,
    }

    TextMeshProUGUI _nameText;
    TextMeshProUGUI _descriptionText;
    TextMeshProUGUI _buttonText;
    TextMeshProUGUI _buttonLevel;

    Button _upgradeButton;

    private Image _upgradeImage;

    UpgradeType _upgradeType;
    int _index;
    int _cost;

    void Awake()
    {
        this.BindUI();
    }

    void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        this._nameText = Get<TextMeshProUGUI>((int)Texts.Text_Name);
        this._descriptionText = Get<TextMeshProUGUI>((int)Texts.Text_Description);
        this._buttonText = Get<TextMeshProUGUI>((int)Texts.Text_Button);
        this._buttonLevel = Get<TextMeshProUGUI>((int)Texts.Text_Level);

        this._upgradeButton = Get<Button>((int)Buttons.Button_Upgrade);
        this._upgradeButton.onClick.AddListener(this.OnClickUpgrade);

        this._upgradeImage = Get<Image>((int)Images.Image_Upgrade);
    }

    public void Initialize(int idx, UpgradeData upgradeData)
    {
        this._upgradeType = upgradeData.Type;
        this._index = idx;
        this._cost = upgradeData.Cost;
        
        this._nameText.text = upgradeData.Name;
        this._descriptionText.text = upgradeData.Description;
        this._buttonText.text = $"{upgradeData.Cost} 골드";
        this._buttonLevel.text = $"Lv{upgradeData.Level}";

        this._upgradeImage.sprite = upgradeData.Icon;

        this.UpdateButtonInteractable();
    }

    void UpdateButtonInteractable()
    {
        if (!HasEnoughGold())
        {
            Debug.Log("222");
            this._upgradeButton.interactable = false;
            return;
        }
        
        switch (this._upgradeType)
        {
            case UpgradeType.Space:
                this._upgradeButton.interactable = UpgradeManager.Instance.CanUpgradeSpace(this._index);
                break;
            case UpgradeType.Speed:
                this._upgradeButton.interactable = UpgradeManager.Instance.CanUpgradeSpeed(this._index);
                break;
            case UpgradeType.OptCost:
                this._upgradeButton.interactable = UpgradeManager.Instance.CanUpgradeOptCostLevel(this._index);
                break;
        }
        
        Debug.Log("333");
    }

    bool HasEnoughGold()
    {
        Debug.Log($"HasEnoughGold(): {GoldManager.Instance.CurrentGold}");
        return GoldManager.Instance.CurrentGold >= this._cost;
    }

    void OnClickUpgrade()
    {
        if (GoldManager.Instance.CurrentGold < this._cost)
            return;
        int chargeCost = _cost;
        Debug.Log($"Upgrade {this._upgradeType}");
        switch (this._upgradeType)
        {
            case UpgradeType.Space:
                UpgradeManager.Instance.UpgradeSpace(this._index);
                break;
            case UpgradeType.Speed:
                UpgradeManager.Instance.UpgradeSpeed(this._index);
                break;
            case UpgradeType.OptCost:
                UpgradeManager.Instance.UpgradeOptCostLevel(this._index);
                break;
        }
        
        GoldChangedEvent.Trigger(
            0,
            chargeCost,
            GoldManager.Instance.CurrentGold,
            GoldManager.Instance.CurrentGold - chargeCost
        );
        GoldManager.Instance.CurrentGold -= chargeCost;
    }
}