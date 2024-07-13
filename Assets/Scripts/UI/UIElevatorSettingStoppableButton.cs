using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIElevatorSettingStoppableButton : MonoBehaviour
{
    public enum ElevatorSettingStoppableButtonType
    {
        Stoppable,
        Pass,
    }

    public ElevatorSettingStoppableButtonType Type = ElevatorSettingStoppableButtonType.Stoppable;
    public Sprite stoppableSprite;
    public Sprite passSprite;
    // 0부터 시작
    public int floor;

    private Image buttonImage;
    private Button button;
    private int elevatorIndex = 0;

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

    public void SetElevatorIndex(int index)
    {
        elevatorIndex = index;
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

        if (elevatorIndex >= ElevatorManager.Instance._elevators.Count)
        {
            Debug.LogWarning($"UIElevatorSettingStoppableButton: elevatorIndex is out of range {elevatorIndex}");
            return;
        }
        var elevatorController = ElevatorManager.Instance._elevators[elevatorIndex];
        var floor = FloorManager.Instance.Floors.FirstOrDefault(f => f.FloorIdx == this.floor);
        if (floor == null) {
            Debug.LogError($"floor not found {this.floor}");
            Debug.LogError(FloorManager.Instance.Floors);
            foreach (var floor_ in FloorManager.Instance.Floors)
            {
                Debug.LogError($"flor {floor_.FloorIdx} : {floor_}");
            }
        }
        switch (Type)
        {
            case ElevatorSettingStoppableButtonType.Stoppable:
                elevatorController.AddStoppableFloor(floor);
                break;
            case ElevatorSettingStoppableButtonType.Pass:
                elevatorController.RemoveStoppableFloor(floor);
                break;
        }
    }
}
