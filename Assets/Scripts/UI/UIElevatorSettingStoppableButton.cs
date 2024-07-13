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
    public Image buttonImage;
    public Sprite stoppableSprite;
    public Sprite passSprite;
    // 0부터 시작
    public int floor;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("UIElevatorSettingStoppableButton: buttonImage is not set");
            return;
        }

        Change(this.Type);
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
