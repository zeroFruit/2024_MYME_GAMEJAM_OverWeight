using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElevatorSettingStoppableButton : MonoBehaviour
{
    public enum ElevatorSettingStoppableButtonType
    {
        Stoppable,
        Pass,
    }

    public ElevatorSettingStoppableButtonType Type = ElevatorSettingStoppableButtonType.Pass;
    public Sprite stoppableSprite;
    public Sprite passSprite;
    // 0부터 시작
    public int floor;

    private Image buttonImage;
    private Button button;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("UIElevatorSettingStoppableButton: buttonImage is not set");
            return;
        }
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("UIElevatorSettingStoppableButton: button is not set");
            return;
        }

        button.onClick.AddListener(OnClick);

        Change(this.Type);
    }

    private void OnClick()
    {
        switch (Type)
        {
            case ElevatorSettingStoppableButtonType.Stoppable:
                Type = ElevatorSettingStoppableButtonType.Pass;
                break;
            case ElevatorSettingStoppableButtonType.Pass:
                Type = ElevatorSettingStoppableButtonType.Stoppable;
                break;
        }
        Change(Type);
    }

    public void Change(ElevatorSettingStoppableButtonType type)
    {
        Type = type;
        switch (Type)
        {
            case ElevatorSettingStoppableButtonType.Stoppable:
                buttonImage.sprite = stoppableSprite;
                break;
            case ElevatorSettingStoppableButtonType.Pass:
                buttonImage.sprite = passSprite;
                break;
        }
    }
}
