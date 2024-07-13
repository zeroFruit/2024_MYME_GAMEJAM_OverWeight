using TMPro;
using UnityEngine;

public class HUDDay : MonoBehaviour
{
    public TMP_Text dayText;

    // Update is called once per frame
    void Update()
    {
        var day = DayManager.Instance.Day;
        dayText.text = "Day " + (day+1).ToString();
    }
}
