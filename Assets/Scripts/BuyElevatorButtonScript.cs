using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyElevatorButtonScript : MonoBehaviour
{
    public int BuyIdx;
    public int Cost;
    private Button Button;

    void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnBuyClick);
    }

    void OnBuyClick()
    {
        if (GoldManager.Instance.CurrentGold < Cost)
            return;
        GoldChangedEvent.Trigger(
            0,
            Cost,
            GoldManager.Instance.CurrentGold,
            GoldManager.Instance.CurrentGold - Cost
        );
        GoldManager.Instance.CurrentGold -= Cost;
        ElevatorManager.Instance.AddElevator(BuyIdx);
        gameObject.SetActive(false);
    }

}