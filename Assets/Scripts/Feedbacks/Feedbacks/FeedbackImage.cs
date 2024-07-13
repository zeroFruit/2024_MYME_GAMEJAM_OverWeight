using System.Collections;
using HAWStudio.Common;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
    /// This feedback will let you change the color of a target sprite renderer over time, and flip it on X or Y. You can also use it to command one or many MMSpriteRendererShakers.
    /// </summary>
    public class FeedbackImage : Feedback
    {
        public enum Modes { OverTime, Instant, ShakerEvent }
        
        [Header("Sprite Renderer")]
        [Tooltip("the Image to affect when playing the feedback")]
        public Image BoundImage;
        [Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
        public Modes Mode = Modes.OverTime;

        [Tooltip("how long the Image should change over time")]
        [FEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float Duration = 0.2f;
        [Tooltip("whether or not that Image should be turned off on start")]
        public bool StartsOff = false;

        [Tooltip("the transform to use to broadcast the event as origin point")]
        [FEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public Transform EventOriginTransform;
        [Tooltip("if this is true, the target will be disabled when this feedbacks is stopped")]
        public bool DisableOnStop = true;
        [Tooltip(
            "if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
        public bool AllowAdditivePlays = false;
        
        [Header("Color")]
        [Tooltip("whether or not to modify the color of the image")]
        public bool ModifyColor = true;
        [Tooltip("the colors to apply to the Image over time")]
        [FEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public Gradient ColorOverTime;
        [Tooltip("the color to move to in instant mode")]
        [FEnumCondition("Mode", (int)Modes.Instant, (int)Modes.ShakerEvent)]
        public Color InstantColor;

        public override float FeedbackDuration
        {
            get => Mode == Modes.Instant ? 0f : this.Duration;
            set => this.Duration = value;
        }

        Coroutine _coroutine;

        protected override void CustomInitialization(FeedbackPlayer owner)
        {
            base.CustomInitialization(owner);

            if (this.EventOriginTransform == null)
            {
                this.EventOriginTransform = this.transform;
            }

            if (this.Active && this.StartsOff)
            {
                Turn(false);
            }
        }
        
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!this.Active)
            {
                return;
            }
            
            Turn(true);
            switch (this.Mode)
            {
                case Modes.Instant:
                    if (this.ModifyColor)
                    {
                        this.BoundImage.color = this.InstantColor;
                    }
                    break;
                case Modes.OverTime:
                    if (!AllowAdditivePlays && this._coroutine != null)
                    {
                        return;
                    }

                    this._coroutine = StartCoroutine(ImageSequence());
                    break;
            }
        }

        IEnumerator ImageSequence()
        {
            float journey = FeedbackDuration;

            IsPlaying = true;
            while (journey >= 0 && journey <= FeedbackDuration && FeedbackDuration > 0)
            {
                float remappedTime = FeedbackHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
                SetImageValues(remappedTime);

                journey -= FeedbackDeltaTime;
                yield return null;
            }
            
            SetImageValues(FinalNormalizedTime);
            if (this.StartsOff)
            {
                Turn(false);
            }

            IsPlaying = false;
            this._coroutine = null;
            yield return null;
        }

        void SetImageValues(float time)
        {
            if (this.ModifyColor)
            {
                this.BoundImage.color = this.ColorOverTime.Evaluate(time);
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!this.Active)
            {
                return;
            }

            IsPlaying = false;
            base.CustomStopFeedback(position, feedbacksIntensity);
            if (this.Active && this.DisableOnStop)
            {
                Turn(false);
            }

            this._coroutine = null;
        }

        /// <summary>
        /// Turns the sprite renderer on or off
        /// </summary>
        /// <param name="status"></param>
        void Turn(bool status)
        {
            this.BoundImage.gameObject.SetActive(status);
            this.BoundImage.enabled = status;
        }
    }