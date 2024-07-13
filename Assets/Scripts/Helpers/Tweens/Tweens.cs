using System.Collections;
using UnityEngine;

/// <summary>
    /// The formulas described here are (loosely) based on Robert Penner's easing equations http://robertpenner.com/easing/
    /// </summary>
    public class Tweens : MonoBehaviour
    {
        /// <summary>
		/// A list of all the possible curves you can tween a value along
		/// </summary>
		public enum TweenCurve
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			EaseInBack,			EaseOutBack,		EaseInOutBack,
			AntiLinearTween,    AlmostIdentity
		}
		
		public static TweenDelegate[] TweenDelegateArray = new TweenDelegate[]
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			EaseInBack,			EaseOutBack,		EaseInOutBack,
			AntiLinearTween,    AlmostIdentity
		};

		// Core methods ---------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Moves a value between a startValue and an endValue based on a currentTime, along the specified tween curve
		/// </summary>
		/// <param name="currentTime"></param>
		/// <param name="initialTime"></param>
		/// <param name="endTime"></param>
		/// <param name="startValue"></param>
		/// <param name="endValue"></param>
		/// <param name="curve"></param>
		/// <returns></returns>
		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenCurve curve)
		{
			currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = TweenDelegateArray[(int)curve](currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static float Evaluate(float t, TweenCurve curve)
		{
			return TweenDelegateArray[(int)curve](t);
		}

		public static float Evaluate(float t, TweenType tweenType)
		{
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
			{
				return Evaluate(t, tweenType.TweenCurve);
			}
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
			{
				return tweenType.Curve.Evaluate(t);
			}
			return 0f;
		}

		public delegate float TweenDelegate(float currentTime);
		
		public static float LinearTween(float currentTime) { return TweenDefinitions.Linear_Tween(currentTime); }
		public static float AntiLinearTween(float currentTime) { return TweenDefinitions.LinearAnti_Tween(currentTime); }
		public static float EaseInQuadratic(float currentTime) { return TweenDefinitions.EaseIn_Quadratic(currentTime); }
		public static float EaseOutQuadratic(float currentTime) { return TweenDefinitions.EaseOut_Quadratic(currentTime); }
		public static float EaseInOutQuadratic(float currentTime) { return TweenDefinitions.EaseInOut_Quadratic(currentTime); }
		public static float EaseInCubic(float currentTime) { return TweenDefinitions.EaseIn_Cubic(currentTime); }
		public static float EaseOutCubic(float currentTime) { return TweenDefinitions.EaseOut_Cubic(currentTime); }
		public static float EaseInOutCubic(float currentTime) { return TweenDefinitions.EaseInOut_Cubic(currentTime); }
		public static float EaseInQuartic(float currentTime) { return TweenDefinitions.EaseIn_Quartic(currentTime); }
		public static float EaseOutQuartic(float currentTime) { return TweenDefinitions.EaseOut_Quartic(currentTime); }
		public static float EaseInOutQuartic(float currentTime) { return TweenDefinitions.EaseInOut_Quartic(currentTime); }
		public static float EaseInQuintic(float currentTime) { return TweenDefinitions.EaseIn_Quintic(currentTime); }
		public static float EaseOutQuintic(float currentTime) { return TweenDefinitions.EaseOut_Quintic(currentTime); }
		public static float EaseInOutQuintic(float currentTime) { return TweenDefinitions.EaseInOut_Quintic(currentTime); }
		public static float EaseInSinusoidal(float currentTime) { return TweenDefinitions.EaseIn_Sinusoidal(currentTime); }
		public static float EaseOutSinusoidal(float currentTime) { return TweenDefinitions.EaseOut_Sinusoidal(currentTime); }
		public static float EaseInOutSinusoidal(float currentTime) { return TweenDefinitions.EaseInOut_Sinusoidal(currentTime); }
		public static float EaseInBounce(float currentTime) { return TweenDefinitions.EaseIn_Bounce(currentTime); }
		public static float EaseOutBounce(float currentTime) { return TweenDefinitions.EaseOut_Bounce(currentTime); }
		public static float EaseInOutBounce(float currentTime) { return TweenDefinitions.EaseInOut_Bounce(currentTime); }
		public static float EaseInOverhead(float currentTime) { return TweenDefinitions.EaseIn_Overhead(currentTime); }
		public static float EaseOutOverhead(float currentTime) { return TweenDefinitions.EaseOut_Overhead(currentTime); }
		public static float EaseInOutOverhead(float currentTime) { return TweenDefinitions.EaseInOut_Overhead(currentTime); }
		public static float EaseInExponential(float currentTime) { return TweenDefinitions.EaseIn_Exponential(currentTime); }
		public static float EaseOutExponential(float currentTime) { return TweenDefinitions.EaseOut_Exponential(currentTime); }
		public static float EaseInOutExponential(float currentTime) { return TweenDefinitions.EaseInOut_Exponential(currentTime); }
		public static float EaseInElastic(float currentTime) { return TweenDefinitions.EaseIn_Elastic(currentTime); }
		public static float EaseOutElastic(float currentTime) { return TweenDefinitions.EaseOut_Elastic(currentTime); }
		public static float EaseInOutElastic(float currentTime) { return TweenDefinitions.EaseInOut_Elastic(currentTime); }
		public static float EaseInCircular(float currentTime) { return TweenDefinitions.EaseIn_Circular(currentTime); }
		public static float EaseOutCircular(float currentTime) { return TweenDefinitions.EaseOut_Circular(currentTime); }
		public static float EaseInOutCircular(float currentTime) { return TweenDefinitions.EaseInOut_Circular(currentTime); }
		public static float EaseInBack(float currentTime) { return TweenDefinitions.EaseIn_Back(currentTime); }
		public static float EaseOutBack(float currentTime) { return TweenDefinitions.EaseOut_Back(currentTime); }
		public static float EaseInOutBack(float currentTime) { return TweenDefinitions.EaseInOut_Back(currentTime); }
		public static float AlmostIdentity(float currentTime) { return TweenDefinitions.AlmostIdentity(currentTime); }

		/// <summary>
		/// To use :
		/// public Tween.TweenCurve Tween = Tween.TweenCurve.EaseInOutCubic;
		/// private Tween.TweenDelegate _tween;
		///
		/// _tween = Tween.GetTweenMethod(Tween);
		/// float t = _tween(someFloat);
		/// </summary>
		/// <param name="tween"></param>
		/// <returns></returns>
		public static TweenDelegate GetTweenMethod(TweenCurve tween)
		{
			switch (tween)
			{
				case TweenCurve.LinearTween: return LinearTween;
				case TweenCurve.AntiLinearTween: return AntiLinearTween;
				case TweenCurve.EaseInQuadratic: return EaseInQuadratic;
				case TweenCurve.EaseOutQuadratic: return EaseOutQuadratic;
				case TweenCurve.EaseInOutQuadratic: return EaseInOutQuadratic;
				case TweenCurve.EaseInCubic: return EaseInCubic;
				case TweenCurve.EaseOutCubic: return EaseOutCubic;
				case TweenCurve.EaseInOutCubic: return EaseInOutCubic;
				case TweenCurve.EaseInQuartic: return EaseInQuartic;
				case TweenCurve.EaseOutQuartic: return EaseOutQuartic;
				case TweenCurve.EaseInOutQuartic: return EaseInOutQuartic;
				case TweenCurve.EaseInQuintic: return EaseInQuintic;
				case TweenCurve.EaseOutQuintic: return EaseOutQuintic;
				case TweenCurve.EaseInOutQuintic: return EaseInOutQuintic;
				case TweenCurve.EaseInSinusoidal: return EaseInSinusoidal;
				case TweenCurve.EaseOutSinusoidal: return EaseOutSinusoidal;
				case TweenCurve.EaseInOutSinusoidal: return EaseInOutSinusoidal;
				case TweenCurve.EaseInBounce: return EaseInBounce;
				case TweenCurve.EaseOutBounce: return EaseOutBounce;
				case TweenCurve.EaseInOutBounce: return EaseInOutBounce;
				case TweenCurve.EaseInOverhead: return EaseInOverhead;
				case TweenCurve.EaseOutOverhead: return EaseOutOverhead;
				case TweenCurve.EaseInOutOverhead: return EaseInOutOverhead;
				case TweenCurve.EaseInExponential: return EaseInExponential;
				case TweenCurve.EaseOutExponential: return EaseOutExponential;
				case TweenCurve.EaseInOutExponential: return EaseInOutExponential;
				case TweenCurve.EaseInElastic: return EaseInElastic;
				case TweenCurve.EaseOutElastic: return EaseOutElastic;
				case TweenCurve.EaseInOutElastic: return EaseInOutElastic;
				case TweenCurve.EaseInCircular: return EaseInCircular;
				case TweenCurve.EaseOutCircular: return EaseOutCircular;
				case TweenCurve.EaseInOutCircular: return EaseInOutCircular;
				case TweenCurve.EaseInBack: return EaseInBack;
				case TweenCurve.EaseOutBack: return EaseOutBack;
				case TweenCurve.EaseInOutBack: return EaseInOutBack;
				case TweenCurve.AlmostIdentity: return AlmostIdentity;
			}
			return LinearTween;
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Animation curve methods --------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
		{
			currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = curve.Evaluate(currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Tween type methods ------------------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenType tweenType)
		{
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
			}
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return 0f;
		}
		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenType tweenType)
		{
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
			}
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector2.zero;
		}
		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenType tweenType)
		{
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
			}
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector3.zero;
		}
		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenType tweenType)
		{
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
			}
			if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Quaternion.identity;
		}

		// MOVE METHODS ---------------------------------------------------------------------------------------------------------
		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination, 
			WaitForSeconds delay, float delayDuration, float duration, Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
			WaitForSeconds delay, float delayDuration, float duration, Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			Tweens.TweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
		}

		public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, 
			float duration, Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
			float delayDuration, float duration, Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.localPosition = Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.localPosition = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay, 
			float delayDuration, float duration, Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.transform.position = Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			Tweens.TweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				if (updatePosition)
				{
					targetTransform.transform.position = Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
				}
				if (updateRotation)
				{
					targetTransform.transform.rotation = Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
				}
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			if (updatePosition) { targetTransform.transform.position = destination.position; }
			if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
		}

		protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, 
			Tweens.TweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}

			Vector3 initialRotationPosition = targetTransform.transform.position;
			Quaternion initialRotationRotation = targetTransform.transform.rotation;

			float rate = 1f / duration;

			float timeSpent = 0f;
			while (timeSpent < duration)
			{

				float newAngle = Tween(timeSpent, 0f, duration, 0f, angle, curve);

				targetTransform.transform.position = initialRotationPosition;
				initialRotationRotation = targetTransform.transform.rotation;
				targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
				targetTransform.transform.rotation = initialRotationRotation;

				timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination.position;
		}
    }