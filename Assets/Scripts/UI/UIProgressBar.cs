using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
    {
        public enum ProgressBarStates {Idle, Decreasing, Increasing, InDecreasingDelay, InIncreasingDelay }
        /// the possible fill modes 
        public enum FillModes { LocalScale, FillAmount, Width, Height, Anchor }
        /// the possible directions for the fill (for local scale and fill amount only)
        public enum BarDirections { LeftToRight, RightToLeft, UpToDown, DownToUp }
        /// the possible timescales the bar can work on
        public enum TimeScales { UnscaledTime, Time }
        /// the possible ways to animate the bar fill
        public enum BarFillModes { SpeedBased, FixedDuration }
        
        [Header("Bindings")]
        [Tooltip("optional - the ID of the player associated to this bar")]
        public string PlayerID;
        [Tooltip("the main, foreground bar")]
        public Transform ForegroundBar;
        [Tooltip("the delayed bar that will show when moving from a value to a new, lower value")]
        public Transform DelayedBarDecreasing;
        [Tooltip("the delayed bar that will show when moving from a value to a new, higher value")]
        public Transform DelayedBarIncreasing;
        
        [Header("Fill Settings")]
        [Range(0f,1f)]
        [Tooltip("the local scale or fillamount value to reach when the value associated to the bar is at 0%")]
        public float MinimumBarFillValue = 0f;
        [Range(0f,1f)]
        [Tooltip("the local scale or fillamount value to reach when the bar is full")]
        public float MaximumBarFillValue = 1f;
        [Tooltip("whether or not to initialize the value of the bar on start")]
        public bool SetInitialFillValueOnStart = false;
        [Condition("SetInitialFillValueOnStart", true)]
        [Range(0f,1f)]
        [Tooltip("the initial value of the bar")]
        public float InitialFillValue = 0f;
        [Tooltip("the direction this bar moves to")]
        public BarDirections BarDirection = BarDirections.LeftToRight;
        [Tooltip("the foreground bar's fill mode")]
        public FillModes FillMode = FillModes.LocalScale;
        [Tooltip("defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)")]
        public TimeScales TimeScale = TimeScales.UnscaledTime;
        [Tooltip("the selected fill animation mode")]
        public BarFillModes BarFillMode = BarFillModes.SpeedBased;
        
        [Header("Foreground Bar Settings")]
        [Tooltip("whether or not the foreground bar should lerp")]
        public bool LerpForegroundBar = true;
        [Tooltip("the speed at which to lerp the foreground bar")]
        [Condition("LerpForegroundBar", true)]
        public float LerpForegroundBarSpeedDecreasing = 15f;
        [Tooltip("the speed at which to lerp the foreground bar if value is increasing")]
        [Condition("LerpForegroundBar", true)]
        public float LerpForegroundBarSpeedIncreasing = 15f;
        [Tooltip("the speed at which to lerp the foreground bar if speed is decreasing")]
        [Condition("LerpForegroundBar", true)]
        public float LerpForegroundBarDurationDecreasing = 0.2f;
        [Tooltip("the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)")]
        [Condition("LerpForegroundBar", true)]
        public float LerpForegroundBarDurationIncreasing = 0.2f;
        [Tooltip("the curve to use when animating the foreground bar fill decreasing")]
        [Condition("LerpForegroundBar", true)]
        public AnimationCurve LerpForegroundBarCurveDecreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Tooltip("the curve to use when animating the foreground bar fill increasing")]
        [Condition("LerpForegroundBar", true)]
        public AnimationCurve LerpForegroundBarCurveIncreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Delayed Bar Decreasing")]
        [Tooltip("the delay before the delayed bar moves (in seconds)")]
        public float DecreasingDelay = 1f;
        [Tooltip("whether or not the delayed bar's animation should lerp")]
        public bool LerpDecreasingDelayedBar = true;
        [Tooltip("the speed at which to lerp the delayed bar")]
        [Condition("LerpDecreasingDelayedBar", true)]
        public float LerpDecreasingDelayedBarSpeed = 15f;
        [Tooltip("the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)")]
        [Condition("LerpDecreasingDelayedBar", true)]
        public float LerpDecreasingDelayedBarDuration = 0.2f;
        [Tooltip("the curve to use when animating the delayed bar fill")]
        [Condition("LerpDecreasingDelayedBar", true)]
        public AnimationCurve LerpDecreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Delayed Bar Increasing")]
        [Tooltip("the delay before the delayed bar moves (in seconds)")]
        public float IncreasingDelay = 1f;
        /// whether or not the delayed bar's animation should lerp
        [Tooltip("whether or not the delayed bar's animation should lerp")]
        public bool LerpIncreasingDelayedBar = true;
        
        [Header("Bump")]
        [Tooltip("whether or not the bar should 'bump' when changing value")]
        public bool BumpScaleOnChange = true;
        [Tooltip("whether or not the bar should bump when its value increases")]
        public bool BumpOnIncrease = false;
        [Tooltip("whether or not the bar should bump when its value decreases")]
        public bool BumpOnDecrease = false;
        [Tooltip("the duration of the bump animation")]
        public float BumpDuration = 0.2f;
        [Tooltip("whether or not the bar should flash when bumping")]
        public bool ChangeColorWhenBumping = true;
        [Tooltip("whether or not to store the initial bar color before a bump")]
        public bool StoreBarColorOnPlay = true;
        [Tooltip("the color to apply to the bar when bumping")]
        [Condition("ChangeColorWhenBumping", true)]
        public Color BumpColor = Color.white;
        [Tooltip("the curve to map the bump animation on")]
        public AnimationCurve BumpScaleAnimationCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        [Tooltip("the curve to map the bump animation color animation on")]
        public AnimationCurve BumpColorAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// whether or not the bar is bumping right now
        public bool Bumping { get; protected set; }
        
        [Header("Events")]
        [Tooltip("an event to trigger every time the bar bumps")]
        public UnityEvent OnBump;
        [Tooltip("an event to trigger every time the bar starts decreasing")]
        public UnityEvent OnBarMovementDecreasingStart;
        [Tooltip("an event to trigger every time the bar stops decreasing")]
        public UnityEvent OnBarMovementDecreasingStop;
        [Tooltip("an event to trigger every time the bar starts increasing")]
        public UnityEvent OnBarMovementIncreasingStart;
        [Tooltip("an event to trigger every time the bar stops increasing")]
        public UnityEvent OnBarMovementIncreasingStop;
        
        [Header("Text")]
        [Tooltip("a TMPro text object to update with the bar's value")]
        public TMP_Text PercentageTextMeshPro;
        /// a prefix to always add to the bar's value display
        [Tooltip("a prefix to always add to the bar's value display")]
        public string TextPrefix;
        /// a suffix to always add to the bar's value display
        [Tooltip("a suffix to always add to the bar's value display")]
        public string TextSuffix;
        /// a value multiplier to always apply to the bar's value when displaying it
        [Tooltip("a value multiplier to always apply to the bar's value when displaying it")]
        public float TextValueMultiplier = 1f;
        /// the format in which the text should display
        [Tooltip("the format in which the text should display")]
        public string TextFormat = "{000}";
        /// whether or not to display the total after the current value 
        [Tooltip("whether or not to display the total after the current value")]
        public bool DisplayTotal = false;
        /// if DisplayTotal is true, the separator to put between the current value and the total
        [Tooltip("if DisplayTotal is true, the separator to put between the current value and the total")]
        // [Condition("DisplayTotal", true)]
        public string TotalSeparator = " / ";
        
        [Header("Debug")]
        [Tooltip("the value the bar will move to if you press the DebugSet button")]
        [Range(0f, 1f)] 
        public float DebugNewTargetValue;

        [InspectorButton("DebugUpdateBar")]
        public bool DebugUpdateBarButton;
        [InspectorButton("DebugSetBar")]
        public bool DebugSetBarButton;
        [InspectorButton("Bump")]
        public bool TestBumpButton;
        [InspectorButton("Plus10Percent")]
        public bool Plus10PercentButton;
        [InspectorButton("Minus10Percent")]
        public bool Minus10PercentButton;
     
        [Tooltip("the current progress of the bar, ideally read only")]
        [Range(0f,1f)]
        public float BarProgress;
        [Tooltip("the current progress of the bar, ideally read only")]
        [Range(0f,1f)]
        public float BarTarget;
        [Tooltip("the current progress of the delayed bar increasing")]
        [Range(0f,1f)]
        public float DelayedBarIncreasingProgress;
        [Tooltip("the current progress of the delayed bar decreasing")]
        [Range(0f,1f)]
        public float DelayedBarDecreasingProgress;
        
        protected bool _initialized;
        protected Vector2 _initialBarSize;
        protected Color _initialColor;
        protected Vector3 _initialScale;
        protected Image _foregroundImage;
        protected Image _delayedDecreasingImage;
        protected Image _delayedIncreasingImage;
        protected Vector3 _targetLocalScale = Vector3.one;
        protected float _newPercent;
        protected float _percentLastTimeBarWasUpdated;
        protected float _lastUpdateTimestamp;
        protected float _time;
        protected float _deltaTime;
        protected int _direction;
        protected Coroutine _coroutine;
        protected bool _coroutineShouldRun = false;
        protected bool _isDelayedBarIncreasingNotNull;
        protected bool _isDelayedBarDecreasingNotNull;
        protected bool _actualUpdate;
        protected Vector2 _anchorVector;
        protected float _delayedBarDecreasingProgress;
        protected float _delayedBarIncreasingProgress;
        protected ProgressBarStates CurrentState = ProgressBarStates.Idle;
        protected string _updatedText;
        protected string _totalText;
        protected bool _isForegroundBarNotNull;
        protected bool _isForegroundImageNotNull;
        protected bool _isPercentageTextMeshProNotNull;

        #region Initialization

        /// <summary>
        /// On start we store our image component
        /// </summary>
        protected virtual void Start()
        {
	        Initialization();
        }

        protected virtual void OnEnable()
        {
	        if (!_initialized)
	        {
		        return;
	        }

	        StoreInitialColor();
        }

        public virtual void Initialization()
        {
	        _isForegroundBarNotNull = ForegroundBar != null;
	        _isDelayedBarDecreasingNotNull = DelayedBarDecreasing != null;
	        _isDelayedBarIncreasingNotNull = DelayedBarIncreasing != null;
			_isPercentageTextMeshProNotNull = PercentageTextMeshPro != null;
	        _initialScale = this.transform.localScale;

	        if (_isForegroundBarNotNull)
	        {
		        _foregroundImage = ForegroundBar.GetComponent<Image>();
		        _isForegroundImageNotNull = _foregroundImage != null;
		        _initialBarSize = _foregroundImage.rectTransform.sizeDelta;
	        }
	        if (_isDelayedBarDecreasingNotNull)
	        {
		        _delayedDecreasingImage = DelayedBarDecreasing.GetComponent<Image>();
	        }
	        if (_isDelayedBarIncreasingNotNull)
	        {
		        _delayedIncreasingImage = DelayedBarIncreasing.GetComponent<Image>();
	        }
	        _initialized = true;

	        StoreInitialColor();

	        _percentLastTimeBarWasUpdated = BarProgress;

	        if (SetInitialFillValueOnStart)
	        {
		        SetBar01(InitialFillValue);
	        }
        }

        protected virtual void StoreInitialColor()
        {
	        if (!Bumping && _isForegroundImageNotNull)
	        {
		        _initialColor = _foregroundImage.color;
	        }
        }

        #endregion

        #region Public API

        /// <summary>
		/// Updates the bar's values, using a normalized value
		/// </summary>
		/// <param name="normalizedValue"></param>
		public virtual void UpdateBar01(float normalizedValue) 
		{
			UpdateBar(Mathf.Clamp01(normalizedValue), 0f, 1f);
		}
        
		/// <summary>
		/// Updates the bar's values based on the specified parameters
		/// </summary>
		/// <param name="currentValue">Current value.</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		public virtual void UpdateBar(float currentValue,float minValue,float maxValue) 
		{
			if (!_initialized)
			{
				Initialization();
			}

			if (StoreBarColorOnPlay)
			{
				StoreInitialColor();	
			}

			if (!this.gameObject.activeInHierarchy)
			{
				this.gameObject.SetActive(true);    
			}
            
			_newPercent = Maths.Remap(currentValue, minValue, maxValue, MinimumBarFillValue, MaximumBarFillValue);
	        
			_actualUpdate = (BarTarget != _newPercent);
	        
			if (!_actualUpdate)
			{
				return;
			}
	        
			if (CurrentState != ProgressBarStates.Idle)
			{
				if ((CurrentState == ProgressBarStates.Decreasing) ||
				    (CurrentState == ProgressBarStates.InDecreasingDelay))
				{
					if (_newPercent >= BarTarget)
					{
						StopCoroutine(_coroutine);
						SetBar01(BarTarget);
					}
				}
				if ((CurrentState == ProgressBarStates.Increasing) ||
				    (CurrentState == ProgressBarStates.InIncreasingDelay))
				{
					if (_newPercent <= BarTarget)
					{
						StopCoroutine(_coroutine);
						SetBar01(BarTarget);
					}
				}
			}
	        
			_percentLastTimeBarWasUpdated = BarProgress;
			_delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
			_delayedBarIncreasingProgress = DelayedBarIncreasingProgress;
	        
			BarTarget = _newPercent;
			
			if ((_newPercent != _percentLastTimeBarWasUpdated) && !Bumping)
			{
				Bump();
			}

			DetermineDeltaTime();
			_lastUpdateTimestamp = _time;
	        
			DetermineDirection();
			if (_direction < 0)
			{
				OnBarMovementDecreasingStart?.Invoke();
			}
			else
			{
				OnBarMovementIncreasingStart?.Invoke();
			}
		        
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			_coroutineShouldRun = true;     
		    

			if (this.gameObject.activeInHierarchy)
			{
				_coroutine = StartCoroutine(UpdateBarsCo());
			}
			else
			{
				SetBar(currentValue, minValue, maxValue);
			}

			UpdateText();
		}

		/// <summary>
		/// Sets the bar value to the one specified 
		/// </summary>
		/// <param name="currentValue"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		public virtual void SetBar(float currentValue, float minValue, float maxValue)
		{
			float newPercent = Maths.Remap(currentValue, minValue, maxValue, 0f, 1f);
			SetBar01(newPercent);
		}

		/// <summary>
		/// Sets the bar value to the normalized value set in parameter
		/// </summary>
		/// <param name="newPercent"></param>
		public virtual void SetBar01(float newPercent)
		{
			if (!_initialized)
			{
				Initialization();
			}

			newPercent = Maths.Remap(newPercent, 0f, 1f, MinimumBarFillValue, MaximumBarFillValue);
			BarProgress = newPercent;
			DelayedBarDecreasingProgress = newPercent;
			DelayedBarIncreasingProgress = newPercent;
			//_newPercent = newPercent;
			BarTarget = newPercent;
			_percentLastTimeBarWasUpdated = newPercent;
			_delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
			_delayedBarIncreasingProgress = DelayedBarIncreasingProgress;
			SetBarInternal(newPercent, ForegroundBar, _foregroundImage, _initialBarSize);
			SetBarInternal(newPercent, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
			SetBarInternal(newPercent, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
			UpdateText();
			_coroutineShouldRun = false;
			CurrentState = ProgressBarStates.Idle;
		}

        #endregion
        
        #region Tests

        /// <summary>
        /// This test method, called via the inspector button of the same name, lets you test what happens when you update the bar to a certain value
        /// </summary>
        protected virtual void DebugUpdateBar()
        {
	        this.UpdateBar01(DebugNewTargetValue);
        }
        
        /// <summary>
        /// Test method
        /// </summary>
        protected virtual void DebugSetBar()
        {
	        this.SetBar01(DebugNewTargetValue);
        }

        /// <summary>
        /// Test method
        /// </summary>
        public virtual void Plus10Percent()
        {
	        float newProgress = BarTarget + 0.1f;
	        newProgress = Mathf.Clamp(newProgress, 0f, 1f);
	        UpdateBar01(newProgress);
        }
        
        /// <summary>
        /// Test method
        /// </summary>
        public virtual void Minus10Percent()
        {
	        float newProgress = BarTarget - 0.1f;
	        newProgress = Mathf.Clamp(newProgress, 0f, 1f);
	        UpdateBar01(newProgress);
        }


        #endregion TESTS
        
        protected virtual void UpdateText()
		{
			_updatedText = TextPrefix + (BarTarget * TextValueMultiplier).ToString(TextFormat);
			if (DisplayTotal)
			{
				_updatedText += TotalSeparator + (TextValueMultiplier).ToString(TextFormat);
			}
			_updatedText += TextSuffix;
			if (_isPercentageTextMeshProNotNull)
			{
				PercentageTextMeshPro.text = _updatedText;
			}
		}
        
		/// <summary>
		/// On Update we update our bars
		/// </summary>
		protected virtual IEnumerator UpdateBarsCo()
		{
			while (_coroutineShouldRun)
			{
				DetermineDeltaTime();
				DetermineDirection();
				UpdateBars();
				yield return null;
			}

			CurrentState = ProgressBarStates.Idle;
			yield break;
		}
		
		protected virtual void DetermineDeltaTime()
		{
			_deltaTime = (TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime;
			_time = (TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;
		}

		protected virtual void DetermineDirection()
		{
			_direction = (_newPercent > _percentLastTimeBarWasUpdated) ? 1 : -1;
		}

		/// <summary>
		/// Updates the foreground bar's scale
		/// </summary>
		protected virtual void UpdateBars()
		{
			float newFill;
			float newFillDelayed;
			float t1, t2 = 0f;
			
			// if the value is decreasing
			if (_direction < 0)
			{
				newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedDecreasing, LerpForegroundBarDurationDecreasing, LerpForegroundBarCurveDecreasing, 0f, _percentLastTimeBarWasUpdated, out t1);
				SetBarInternal(newFill, ForegroundBar, _foregroundImage, _initialBarSize);
				SetBarInternal(newFill, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);

				BarProgress = newFill;
				DelayedBarIncreasingProgress = newFill;

				CurrentState = ProgressBarStates.Decreasing;
				
				if (_time - _lastUpdateTimestamp > DecreasingDelay)
				{
					newFillDelayed = ComputeNewFill(LerpDecreasingDelayedBar, LerpDecreasingDelayedBarSpeed, LerpDecreasingDelayedBarDuration, LerpDecreasingDelayedBarCurve, DecreasingDelay,_delayedBarDecreasingProgress, out t2);
					SetBarInternal(newFillDelayed, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);

					DelayedBarDecreasingProgress = newFillDelayed;
					CurrentState = ProgressBarStates.InDecreasingDelay;
				}
			}
			else // if the value is increasing
			{
				newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _delayedBarIncreasingProgress, out t1);
				SetBarInternal(newFill, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
				
				DelayedBarIncreasingProgress = newFill;
				CurrentState = ProgressBarStates.Increasing;

				if (DelayedBarIncreasing == null)
				{
					newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _percentLastTimeBarWasUpdated, out t2);
					SetBarInternal(newFill, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
					SetBarInternal(newFill, ForegroundBar, _foregroundImage, _initialBarSize);
					
					BarProgress = newFill;	
					DelayedBarDecreasingProgress = newFill;
					CurrentState = ProgressBarStates.InDecreasingDelay;
				}
				else
				{
					if (_time - _lastUpdateTimestamp > IncreasingDelay)
					{
						newFillDelayed = ComputeNewFill(LerpIncreasingDelayedBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, IncreasingDelay, _delayedBarDecreasingProgress, out t2);
					
						SetBarInternal(newFillDelayed, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
						SetBarInternal(newFillDelayed, ForegroundBar, _foregroundImage, _initialBarSize);
					
						BarProgress = newFillDelayed;	
						DelayedBarDecreasingProgress = newFillDelayed;
						CurrentState = ProgressBarStates.InDecreasingDelay;
					}
				}
			}
			
			if ((t1 >= 1f) && (t2 >= 1f))
			{
				_coroutineShouldRun = false;
				if (_direction > 0)
				{
					OnBarMovementIncreasingStop?.Invoke();
				}
				else
				{
					OnBarMovementDecreasingStop?.Invoke();
				}
			}
		}

		protected virtual float ComputeNewFill(bool lerpBar, float barSpeed, float barDuration, AnimationCurve barCurve, float delay, float lastPercent, out float t)
		{
			float newFill = 0f;
			t = 0f;
			if (lerpBar)
			{
				float delta = 0f;
				float timeSpent = _time - _lastUpdateTimestamp - delay;
				float speed = barSpeed;
				if (speed == 0f) { speed = 1f; }
				
				float duration = (BarFillMode == BarFillModes.FixedDuration) ? barDuration : (Mathf.Abs(_newPercent - lastPercent)) / speed;
				
				delta = Maths.Remap(timeSpent, 0f, duration, 0f, 1f);
				delta = Mathf.Clamp(delta, 0f, 1f);
				t = delta;
				if (t < 1f)
				{
					delta = barCurve.Evaluate(delta);
					newFill = Mathf.LerpUnclamped(lastPercent, _newPercent, delta);	
				}
				else
				{
					newFill = _newPercent;
				}
			}
			else
			{
				newFill = _newPercent;
			}

			newFill = Mathf.Clamp( newFill, 0f, 1f);

			return newFill;
		}

		protected virtual void SetBarInternal(float newAmount, Transform bar, Image image, Vector2 initialSize)
		{
			if (bar == null)
			{
				return;
			}
			
			switch (FillMode)
			{
				case FillModes.LocalScale:
					_targetLocalScale = Vector3.one;
					switch (BarDirection)
					{
						case BarDirections.LeftToRight:
							_targetLocalScale.x = newAmount;
							break;
						case BarDirections.RightToLeft:
							_targetLocalScale.x = 1f - newAmount;
							break;
						case BarDirections.DownToUp:
							_targetLocalScale.y = newAmount;
							break;
						case BarDirections.UpToDown:
							_targetLocalScale.y = 1f - newAmount;
							break;
					}

					bar.localScale = _targetLocalScale;
					break;

				case FillModes.Width:
					if (image == null)
					{
						return;
					}
					float newSizeX = Maths.Remap(newAmount, 0f, 1f, 0, initialSize.x);
					image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeX);
					break;

				case FillModes.Height:
					if (image == null)
					{
						return;
					}
					float newSizeY = Maths.Remap(newAmount, 0f, 1f, 0, initialSize.y);
					image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeY);
					break;

				case FillModes.FillAmount:
					if (image == null)
					{
						return;
					}
					image.fillAmount = newAmount;
					break;
				case FillModes.Anchor:
					if (image == null)
					{
						return;
					}
					switch (BarDirection)
					{
						case BarDirections.LeftToRight:
							_anchorVector.x = 0f;
							_anchorVector.y = 0f;
							image.rectTransform.anchorMin = _anchorVector;
							_anchorVector.x = newAmount;
							_anchorVector.y = 1f;
							image.rectTransform.anchorMax = _anchorVector;
							break;
						case BarDirections.RightToLeft:
							_anchorVector.x = newAmount;
							_anchorVector.y = 0f;
							image.rectTransform.anchorMin = _anchorVector;
							_anchorVector.x = 1f;
							_anchorVector.y = 1f;
							image.rectTransform.anchorMax = _anchorVector;
							break;
						case BarDirections.DownToUp:
							_anchorVector.x = 0f;
							_anchorVector.y = 0f;
							image.rectTransform.anchorMin = _anchorVector;
							_anchorVector.x = 1f;
							_anchorVector.y = newAmount;
							image.rectTransform.anchorMax = _anchorVector;
							break;
						case BarDirections.UpToDown:
							_anchorVector.x = 0f;
							_anchorVector.y = newAmount;
							image.rectTransform.anchorMin = _anchorVector;
							_anchorVector.x = 1f;
							_anchorVector.y = 1f;
							image.rectTransform.anchorMax = _anchorVector;
							break;
					}
					break;
			}
		}
		
		#region  Bump

		/// <summary>
		/// Triggers a camera bump
		/// </summary>
		public virtual void Bump()
		{
			bool shouldBump = false;

			if (!_initialized)
			{
				return;
			}
			
			DetermineDirection();
			
			if (BumpOnIncrease && (_direction > 0))
			{
				shouldBump = true;
			}
			
			if (BumpOnDecrease && (_direction < 0))
			{
				shouldBump = true;
			}
			
			if (BumpScaleOnChange)
			{
				shouldBump = true;
			}

			if (!shouldBump)
			{
				return;
			}
			
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(BumpCoroutine());
			}

			OnBump?.Invoke();
		}

		/// <summary>
		/// A coroutine that (usually quickly) changes the scale of the bar 
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator BumpCoroutine()
		{
			float journey = 0f;

			Bumping = true;

			while (journey <= BumpDuration)
			{
				journey = journey + _deltaTime;
				float percent = Mathf.Clamp01(journey / BumpDuration);
				float curvePercent = BumpScaleAnimationCurve.Evaluate(percent);
				float colorCurvePercent = BumpColorAnimationCurve.Evaluate(percent);
				this.transform.localScale = curvePercent * _initialScale;

				if (ChangeColorWhenBumping && _isForegroundImageNotNull)
				{
					_foregroundImage.color = Color.Lerp(_initialColor, BumpColor, colorCurvePercent);
				}
				yield return null;
			}
			if (ChangeColorWhenBumping && _isForegroundImageNotNull)
			{
				_foregroundImage.color = _initialColor;
			}
			Bumping = false;
			yield return null;
		}

		#endregion Bump

		#region ShowHide

		/// <summary>
		/// A simple method you can call to show the bar (set active true)
		/// </summary>
		public virtual void ShowBar()
		{
			this.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides (SetActive false) the progress bar object, after an optional delay
		/// </summary>
		/// <param name="delay"></param>
		public virtual void HideBar(float delay)
		{
			if (delay <= 0)
			{
				this.gameObject.SetActive(false);
			}
			else if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(HideBarCo(delay));
			}
		}

		/// <summary>
		/// An internal coroutine used to handle the disabling of the progress bar after a delay
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		protected virtual IEnumerator HideBarCo(float delay)
		{
			yield return CoroutineHelper.WaitFor(delay);
			this.gameObject.SetActive(false);
		}

		#endregion ShowHide
    }