using SSR.OverWeight;
using TMPro;
using UnityEngine;

public class GoldUIScript : MonoBehaviour, EventListener<GoldChangedEvent>
{
    public TMP_Text GoldText;

    void Start()
    {
        GoldText = gameObject.GetComponentInChildren<TMP_Text>();
        GoldText.text = GoldManager.Instance.CurrentGold.ToString();
    }

    public void OnEvent(GoldChangedEvent e)
    {
        GoldText.text = e.To.ToString();
        
        // @roy 이거 옮겨야함 ㅋㅋ
        if (e.To < 0)
        {
            GameOverEvent.Trigger();
        }

    }

    void OnEnable()
    {
        this.StartListeningEvent();
    }

    void OnDisable()
    {
        this.StopListeningEvent();
    }
}