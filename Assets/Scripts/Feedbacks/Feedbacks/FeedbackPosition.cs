using System;
using System.Collections;
using UnityEngine;

namespace HAWStudio.Common
{
    public class FeedbackPosition : Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        
        public enum Spaces { World, Local, RectTransform }
        public enum Modes { AtoB, ToDestination }
        
        [Header("Position Target")]
        [Tooltip("the object this feedback will animate the position for")]
        public GameObject AnimatePositionTarget;
        
        [Header("Transition")]
        [Tooltip("the mode this animation should follow (either going from A to B, or moving along a curve)")]
        public Modes Mode = Modes.AtoB;
        [Tooltip("the space in which to move the position in")]
        public Spaces Space = Spaces.World;
        [Tooltip("the duration of the animation on play")]
        public float AnimatePositionDuration = 0.2f;
        [Tooltip("the MMTween curve definition to use instead of the animation curve to define the acceleration of the movement")]
        [FEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public TweenType AnimatePositionTween = new TweenType( new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
        [Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")] 
        public bool AllowAdditivePlays = false;
        
        [Header("Positions")]
        [Tooltip("if this is true, movement will be relative to the object's initial position. So moving its y position along a curve going from 0 to 1 will move it up one unit. If this is false, in that same example, it'll be moved from 0 to 1 in absolute coordinates.")]
        public bool RelativePosition = true;
        [Tooltip("the initial position")]
        [FEnumCondition("Mode", (int)Modes.AtoB)]
        public Vector3 InitialPosition = Vector3.zero;
        /// the destination position
        [Tooltip("the destination position")]
        [FEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public Vector3 DestinationPosition = Vector3.one;
        [Tooltip("the initial transform - if set, takes precedence over the Vector3 above")]
        [FEnumCondition("Mode", (int)Modes.AtoB)]
        public Transform InitialPositionTransform;
        /// the destination transform - if set, takes precedence over the Vector3 above
        [Tooltip("the destination transform - if set, takes precedence over the Vector3 above")]
        [FEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public Transform DestinationPositionTransform;
        
        public override float FeedbackDuration
        {
            get => this.AnimatePositionDuration;
            set => this.AnimatePositionDuration = value;
        }
        
        RectTransform _rectTransform;
        Vector3 _currentPosition;
        Vector3 _newPosition;
        Vector3 _initialPosition;
        Vector3 _destinationPosition;
        Coroutine _coroutine;
        Vector3 _workInitialPosition;
        Vector3 _workDestinationPosition;

        protected override void CustomInitialization(FeedbackPlayer owner)
        {
            base.CustomInitialization(owner);
            if (!this.Active)
            {
                return;
            }
            
            if (AnimatePositionTarget == null)
            {
                Debug.LogWarning("The animate position target for " + this + " is null, you have to define it in the inspector");
                return;
            }

            if (Space == Spaces.RectTransform)
            {
                _rectTransform = AnimatePositionTarget.GetComponent<RectTransform>();
            }

            DeterminePositions();
        }

        void DeterminePositions()
        {
            if (this.InitialPositionTransform != null)
            {
                this._workInitialPosition = GetPosition(this.InitialPositionTransform);
            }
            else
            {
                this._workInitialPosition = this.RelativePosition
                    ? GetPosition(this.AnimatePositionTarget.transform) + this.InitialPosition
                    : this.InitialPosition;
            }

            if (this.DestinationPositionTransform != null)
            {
                this._workDestinationPosition = GetPosition(this.DestinationPositionTransform);
            }
            else
            {
                this._workDestinationPosition = this.RelativePosition
                    ? GetPosition(this.AnimatePositionTarget.transform) + this.DestinationPosition
                    : this.DestinationPosition;
            }
        }

        /// <summary>
        /// On Play, we move our object from A to B
        /// </summary>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized || (AnimatePositionTarget == null))
            {
                return;
            }
            
            switch (this.Mode)
            {
                case Modes.ToDestination:
                    this._initialPosition = GetPosition(this.AnimatePositionTarget.transform);
                    this._destinationPosition = this._workDestinationPosition;
                    if (this.DestinationPositionTransform != null)
                    {
                        this._destinationPosition = GetPosition(this.DestinationPositionTransform);
                    }

                    this._coroutine = this.Owner.StartCoroutine(MoveFromTo(this.AnimatePositionTarget,
                        this._initialPosition, this._destinationPosition, FeedbackDuration, AnimatePositionTween));
                    break;
                case Modes.AtoB:
                    if (!AllowAdditivePlays && (_coroutine != null))
                    {
                        return;
                    }
                    _coroutine = Owner.StartCoroutine(MoveFromTo(AnimatePositionTarget, _workInitialPosition, _workDestinationPosition, FeedbackDuration, AnimatePositionTween));
                    break;
            }
        }

        /// <summary>
        /// Gets the world, local or anchored position
        /// </summary>
        Vector3 GetPosition(Transform target)
        {
            switch (this.Space)
            {
                case Spaces.World:
                    return target.position;
                case Spaces.Local:
                    return target.localPosition;
                case Spaces.RectTransform:
                    return target.gameObject.GetComponent<RectTransform>().anchoredPosition;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Moves an object from point A to point B in a given time
        /// </summary>
        IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float duration, TweenType tweenType)
        {
            IsPlaying = true;
            float journey = 0f;
            while (journey >= 0 && journey <= duration && duration > 0)
            {
                float curveValue = Tweens.Tween(journey, 0f, duration, 0f, 1f, tweenType);

                this._newPosition = Vector3.LerpUnclamped(pointA, pointB, curveValue);
                SetPosition(movingObject.transform, this._newPosition);

                journey += FeedbackDeltaTime;
                yield return null;
            }
            
            SetPosition(movingObject.transform, pointB);
            
            this._coroutine = null;
            IsPlaying = false;
        }

        void SetPosition(Transform target, Vector3 newPosition)
        {
            switch (this.Space)
            {
                case Spaces.World:
                    target.position = newPosition;
                    break;
                case Spaces.Local:
                    target.localPosition = newPosition;
                    break;
                case Spaces.RectTransform:
                    _rectTransform.anchoredPosition = newPosition;
                    break;
            }
        }

        /// <summary>
        /// On stop, we interrupt movement if it was active
        /// </summary>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized || (_coroutine == null))
            {
                return;
            }
            IsPlaying = false;
            Owner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        void OnDisable()
        {
            _coroutine = null;
        }
    }
}