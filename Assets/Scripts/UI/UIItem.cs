using System;
using TMPro;
using UnityEngine;

public class UIItem : UIBase
{
    public enum Texts
    {
        Text_Name,
        Text_Description,
        Text_Button,
    }

    TextMeshProUGUI _nameText;
    TextMeshProUGUI _descriptionText;
    TextMeshProUGUI _buttonText;

    void Awake()
    {
        this.BindUI();
    }

    void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        this._nameText = Get<TextMeshProUGUI>((int)Texts.Text_Name);
        this._descriptionText = Get<TextMeshProUGUI>((int)Texts.Text_Description);
        this._buttonText = Get<TextMeshProUGUI>((int)Texts.Text_Button);
    }

    public void Initialize(UpgradeData upgradeData)
    {
        this._nameText.text = upgradeData.Name;
        this._descriptionText.text = upgradeData.Description;
        this._buttonText.text = $"{upgradeData.Cost} 골드";
    }
}