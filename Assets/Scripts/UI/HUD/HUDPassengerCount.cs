using TMPro;
using UnityEngine;

public class HUDPassengerCount : MonoBehaviour
{
    public TMP_Text passengerText;

    void Awake() {
        if (passengerText == null) {
            Debug.LogError("HUDPassengerCount: GoldText is not set");
        }
    }
    // Update is called once per frame
    void Update()
    {
        var totalCount = PassengerManager.Instance.totalOffboardingPassengerCount;
        
        passengerText.text = totalCount.ToString();
    }
}
