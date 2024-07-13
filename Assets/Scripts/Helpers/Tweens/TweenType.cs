using System;
using UnityEngine;

public enum TweenDefinitionTypes { Tween, AnimationCurve }
    
[Serializable]
public class TweenType
{
    public TweenDefinitionTypes TweenDefinitionType = TweenDefinitionTypes.Tween;
    public Tweens.TweenCurve TweenCurve = Tweens.TweenCurve.EaseInCubic;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));
    public bool Initialized = false;

    public TweenType(Tweens.TweenCurve newCurve)
    {
        TweenCurve = newCurve;
        TweenDefinitionType = TweenDefinitionTypes.Tween;
    }
    public TweenType(AnimationCurve newCurve)
    {
        Curve = newCurve;
        TweenDefinitionType = TweenDefinitionTypes.AnimationCurve;
    }

    public float Evaluate(float t)
    {
        return Tweens.Evaluate(t, this);
    }
}