using UnityEngine;

public class UIPanel : UIBase
{
    [Header("Bindings")]
    [Tooltip("a canvas group that manage this ui panel")]
    public CanvasGroup CanvasGroup;

    [Header("Fade")] 
    [Tooltip("a duration when this ui panel shows")]
    public float FadeInDuration = 0.2f;
    [Tooltip("a duration when this ui panel hides")]
    public float FadeOutDuration = 0.2f;
        
    public bool Visible { get; protected set; }
    protected Transform _character;

    protected virtual void Awake()
    {
        Visible = false;
    }

    public virtual void Show(Transform character)
    {
        this._character = character;
        Visible = true;
        CanvasGroup.blocksRaycasts = true;
        StartCoroutine(FadeHelper.FadeCanvasGroup(this.CanvasGroup, FadeInDuration, 1f));
    }

    public virtual void Hide()
    {
        this._character = null; 
        Visible = false;
        CanvasGroup.blocksRaycasts = false;
        StartCoroutine(FadeHelper.FadeCanvasGroup(this.CanvasGroup, this.FadeOutDuration, 0f));
    }
}