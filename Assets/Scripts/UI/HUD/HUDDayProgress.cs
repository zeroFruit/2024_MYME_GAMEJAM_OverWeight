using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDDayProgress : MonoBehaviour
{
    public float progress = 0;

    public bool debug = false;
    public UIProgressBar progressBar;

    void Awake() {
        if (progressBar == null) {
            Debug.LogError("HUDDayProgress: progressBar is not set");
        }
    }

    /**
     * progress: 0~1
     */
    public void UpdateDayProgress(float progress)
    {
        if (progress < 0 || progress > 1) {
            Debug.LogError("HUDDayProgress: progress should be 0~1");
            return;
        }
        this.progressBar.UpdateBar01(progress);
    }

    void Update() {
        if (debug) {
            if (progress >= 1) {
                progress = 1;
            }
            if (progress <= 0) {
                progress = 0;
            }
            UpdateDayProgress(progress);
        }
    }
}
