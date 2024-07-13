using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorTimer : MonoBehaviour
{
    private float _duration;
    private bool isInProgress;

    public void Init()
    {
        isInProgress = false;
        transform.localPosition = new Vector3(-31, 0.44f, 0);
        transform.gameObject.SetActive(false);
    }

    public void ResetProgress()
    {
        isInProgress = false;
        gameObject.SetActive(false);
    }

    public void Progress(float duration)
    {
        if (isInProgress)
        {
            return;
        }

        gameObject.SetActive(true);
        isInProgress = true;
        _duration = duration;
        StartCoroutine(ProgressRoutine());
    }

    IEnumerator ProgressRoutine()
    {
        float value = 1f;
        float timer = 0f;
        while (value > 0f)
        {
            timer += Time.deltaTime;
            value = 1 - timer / this._duration;
            int remainTime = Mathf.FloorToInt(value * 10);
            transform.GetComponent<TextMeshPro>().text = "" + remainTime;
            yield return null;
        }

        isInProgress = false;
        // todo : trigger event
        ResetProgress();
    }
}

// public class UIProgressFill : MonoBehaviour
//     {
//         public enum FillTypes { Increasing, Decreasing }
//
//         public FillTypes FillType = FillTypes.Decreasing;
//         
//         public UnityAction OnComplete;
//
//         Image _fill;
//         float _duration;
//         bool _inProgress;
//         bool _ran = false;
//
//         void Awake()
//         {
//             this._fill = this.GetComponent<Image>();
//         }
//
//         public void ResetProgress()
//         {
//             switch (this.FillType)
//             {
//                 case FillTypes.Increasing:
//                     this._fill.fillAmount = 0f;
//                     break;
//                 case FillTypes.Decreasing:
//                     this._fill.fillAmount = 1f;
//                     break;
//             }
//             
//             this._ran = false;
//             this._inProgress = false;
//         }
//
//         public void SetFill(float fillAmount)
//         {
//             this._fill.fillAmount = fillAmount;
//         }
//
//         public bool CanStart()
//         {
//             return !this._ran && !this._inProgress;
//         }
//
//         public void Progress(float duration)
//         {
//             if (_inProgress)
//             {
//                 return;
//             }
//
//             this._inProgress = true;
//             this._duration = duration;
//             StartCoroutine(this.ProgressRoutine());
//         }
//
//         IEnumerator ProgressRoutine()
//         {
//             if (this.FillType == FillTypes.Increasing)
//             {
//                 this._fill.fillAmount = 0f;
//             
//                 float value = 0f;
//                 float timer = 0f;
//                 while (value < 1f)
//                 {
//                     timer += Time.deltaTime;
//                     value = timer / this._duration;
//                     this._fill.fillAmount = value;
//                     yield return null;
//                 }
//             
//                 this._inProgress = false;
//                 this._ran = true;
//             
//                 this.OnComplete?.Invoke();
//             }
//             else
//             {
//                 this._fill.fillAmount = 1f;
//             
//                 float value = 1f;
//                 float timer = 0f;
//                 while (value > 0f)
//                 {
//                     timer += Time.deltaTime;
//                     value = 1 - timer / this._duration;
//                     this._fill.fillAmount = value;
//                     yield return null;
//                 }
//             
//                 this._inProgress = false;
//                 this._ran = true;
//             
//                 this.OnComplete?.Invoke();
//             }
//         }
//     }