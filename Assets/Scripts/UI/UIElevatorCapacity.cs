using TMPro;
using UnityEngine;

public class UIElevatorCapacity : MonoBehaviour
{
    public int elevatorIndex;
    public TMP_Text elevatorCapacityText;
    public void SetElevatorIndex(int index)
    {
        elevatorIndex = index;
    }

    void Update() {
        var elevatorManager = ElevatorManager.Instance;
        if (elevatorManager._elevators.Count <= elevatorIndex) {
            return;
        }
        var elevator = ElevatorManager.Instance._elevators[elevatorIndex];
        elevatorCapacityText.text = elevator._maxCapacity + " 명";
    }
}
