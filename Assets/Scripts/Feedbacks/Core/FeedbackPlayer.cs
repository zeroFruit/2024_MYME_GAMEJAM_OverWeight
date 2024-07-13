using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAWStudio.Common
{
    public class FeedbackPlayer : MonoBehaviour
    {
        #region Properties

        [SerializeReference]
        public List<Feedback> FeedbacksList;
        
        /// the possible initialization modes. If you use Script, you'll have to initialize manually by calling the Initialization method and passing it an owner
        /// Otherwise, you can have this component initialize itself at Awake or Start, and in this case the owner will be the MMFeedbacks itself
        public enum InitializationModes { Script, Awake, Start }
        /// the chosen initialization mode
        [Tooltip("the chosen initialization modes. If you use Script, you'll have to initialize manually by calling the " +
                 "Initialization method and passing it an owner. Otherwise, you can have this component initialize " +
                 "itself at Awake or Start, and in this case the owner will be the MMFeedbacks itself")]
        public InitializationModes InitializationMode = InitializationModes.Start;
        [Tooltip("if you set this to true, the system will make changes to ensure that initialization always happens before play")]
        public bool AutoInitialization = true;
        
        [Tooltip("a time multiplier that will be applied to all feedback durations (initial delay, duration, delay between repeats...)")]
        public float DurationMultiplier = 1f;
        
        [Tooltip("a duration, in seconds, during which triggering a new play of this Feedbacks after it's been played once will be impossible")]
        public float CooldownDuration = 0f;
        [Tooltip("a duration, in seconds, to delay the start of this Feedbacks' contents play")]
        public float InitialDelay = 0f;
        [Tooltip("whether this player can be played or not, useful to temporarily prevent play from another class, for example")]
        public bool CanPlay = true;
        [Tooltip("if this is true, you'll be able to trigger a new Play while this feedback is already playing, otherwise you won't be able to")]
        public bool CanPlayWhileAlreadyPlaying = true;
        
        [Tooltip("a number of UnityEvents that can be triggered at the various stages of this MMFeedbacks")] 
        public FeedbacksEvents Events;
        
        [Tooltip("a global switch used to turn all feedbacks on or off globally")]
        public static bool GlobalMMFeedbacksActive = true;
        
        [Tooltip("how many times this player has started playing")]
        [ReadOnly]
        public int PlayCount = 0;
        
        /// whether or not this Feedbacks is playing right now - meaning it hasn't been stopped yet.
        /// if you don't stop your Feedbacks it'll remain true of course
        public bool IsPlaying { get; protected set; }
        /// the amount of times this Feedbacks has been played
        public int TimesPlayed { get; protected set; }
        
        public float TotalDuration
        {
            get
            {
                return _cachedTotalDuration;
            }
        }

        public float GetTime() => Time.time;
        
        protected const float _smallValue = 0.001f;

        float _startTime = 0f;
        float _holdingMax = 0f;
        float _lastStartAt = -float.MaxValue;
        int _lastStartFrame = -1;
        bool _pauseFound = false;
        float _totalDuration = 0f;
        bool _shouldStop = false;
        float _randomDurationMultiplier = 1f;
        float _cachedTotalDuration;
        bool _initialized;
        
        #endregion

        #region Initialization

        void Awake()
        {
            if (this.InitializationMode == InitializationModes.Awake)
            {
                Initialization();
            }

            InitializeFeedbackList();
            ExtraInitializationChecks();
            ComputeCachedTotalDuration();
            PreInitialization();
        }

        /// <summary>
        /// We initialize our list of feedbacks
        /// </summary>
        void InitializeFeedbackList()
        {
            if (this.FeedbacksList == null)
            {
                this.FeedbacksList = new List<Feedback>();
            }
        }

        /// <summary>
        /// Performs extra checks, mostly to cover cases of dynamic creation
        /// </summary>
        void ExtraInitializationChecks()
        {
            if (Events == null)
            {
                Events = new FeedbacksEvents();
                Events.Initialize();
            }
        }

        void PreInitialization()
        {
            int count = this.FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.FeedbacksList[i] != null)
                {
                    this.FeedbacksList[i].PreInitialization(this, i);
                }
            }
        }

        void Start()
        {
            if (this.InitializationMode == InitializationModes.Start)
            {
                Initialization();
            }
            
            // TODO: auto play?
        }

        void Initialization()
        {
            IsPlaying = false;
            this._lastStartAt = -float.MaxValue;

            int count = this.FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.FeedbacksList[i] != null)
                {
                    this.FeedbacksList[i].Initialization(this, i);
                }
            }

            this._initialized = true;
        }

        #endregion
        
        protected virtual void Update()
        {
            if (_shouldStop)
            {
                if (HasFeedbackStillPlaying())
                {
                    return;
                }
                IsPlaying = false;
                Events.TriggerOnComplete(this);
                // ApplyAutoRevert();
                this.enabled = false;
                _shouldStop = false;
            }
            if (IsPlaying)
            {
                if (GetTime() - _startTime > _totalDuration)
                {
                    _shouldStop = true;
                }   
            }
            else
            {
                this.enabled = false;
            }
        }


        #region Play

        public void PlayFeedbacks()
        {
            this.PlayFeedbacks(this.transform.position);
        }

        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation
        /// </summary>
        public void PlayFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            PlayFeedbacksInternal(this.transform.position, feedbacksIntensity);
        }

        void PlayFeedbacksInternal(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (AutoInitialization)
            {
                if (!_initialized)
                {
                    Initialization();
                }
            }
            
            if (IsPlaying && !this.CanPlayWhileAlreadyPlaying)
            {
                return;
            }

            if (this.CooldownDuration > 0f)
            {
                if (this.GetTime() - this._lastStartAt < this.CooldownDuration)
                {
                    return;
                }
            }

            if (!GlobalMMFeedbacksActive)
            {
                return;
            }

            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }
            
            this.ResetFeedbacks();
            this.enabled = true;
            TimesPlayed++;
            IsPlaying = true;
            this._startTime = this.GetTime();
            this._lastStartAt = this._startTime;
            this._totalDuration = TotalDuration;

            CheckForPauses();

            if (this.InitialDelay > 0f)
            {
                StartCoroutine(this.HandleInitialDelayRoutine(position, feedbacksIntensity));
                return;
            }
            
            PreparePlay(position, feedbacksIntensity);
        }
        
        void PreparePlay(Vector3 position, float feedbacksIntensity)
        {
            _holdingMax = 0f;
            this.CheckForPauses();

            if (!this._pauseFound)
            {
                PlayAllFeedbacks(position, feedbacksIntensity);
                return;
            }

            StartCoroutine(this.PausedFeedbacksRoutine(position, feedbacksIntensity));
        }
        
        void PlayAllFeedbacks(Vector3 position, float feedbacksIntensity)
        {
            for (int i = 0; i < this.FeedbacksList.Count; i++)
            {
                if (CanFeedbackPlay(this.FeedbacksList[i]))
                {
                    this.FeedbacksList[i].Play(position, feedbacksIntensity);
                }
            }
        }
        
        IEnumerator PausedFeedbacksRoutine(Vector3 position, float feedbacksIntensity)
        {
            // TODO: implement me
            IsPlaying = true;

            int i = 0;

            // while (i >= 0 && i < this.FeedbackList.Count)
            // {
            //     if (!IsPlaying)
            //     {
            //         yield break;
            //     }
            //     
            //     // skip: handles holding pauses
            //     // skip: Handles pause
            //     // skip: updates holding max
            //     // skip: handles looper
            // }

            while (HasFeedbackStillPlaying())
            {
                yield return null;
            }

            this.IsPlaying = false;
            this.Events.TriggerOnComplete(this);
        }
        
        bool HasFeedbackStillPlaying()
        {
            int count = this.FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.FeedbacksList[i] != null && this.FeedbacksList[i].IsPlaying)
                {
                    return true;
                }
            }

            return false;
        }
        
        IEnumerator HandleInitialDelayRoutine(Vector3 position, float feedbacksIntensity)
        {
            IsPlaying = true;
            yield return CoroutineHelper.WaitFor(this.InitialDelay);
            PreparePlay(position, feedbacksIntensity);
        }
        
        void CheckForPauses()
        {
            // TODO: implement me when adding pause feature
            this._pauseFound = false;
        }
        
        /// <summary>
        /// Calls each feedback's Reset method if they've defined one. An example of that can be resetting the initial color of a flickering renderer.
        /// </summary>
        void ResetFeedbacks()
        {
            foreach (Feedback feedback in this.FeedbacksList)
            {
                if (feedback != null && feedback.Active)
                {
                    feedback.ResetFeedback();
                }
            }

            IsPlaying = false;
        }

        #endregion

        #region Stop

        /// <summary>
        /// Stops all further feedbacks from playing, as well as stopping individual feedbacks 
        /// </summary>
        public void StopFeedbacks()
        {
            StopFeedbacks(true);
        }

        /// <summary>
        /// Stops all feedbacks from playing, with an option to also stop individual feedbacks
        /// </summary>
        public void StopFeedbacks(bool stopAllFeedbacks = true)
        {
            StopFeedbacks(this.transform.position, 1.0f, stopAllFeedbacks);
        }

        /// <summary>
        /// Stops all feedbacks from playing, specifying a position and intensity that can be used by the Feedbacks 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool stopAllFeedbacks = true)
        {
            if (stopAllFeedbacks)
            {
                int count = FeedbacksList.Count;
                for (int i = 0; i < count; i++)
                {
                    FeedbacksList[i].Stop(position, feedbacksIntensity);
                }    
            }
            IsPlaying = false;
            StopAllCoroutines();
        }

        #endregion

        #region Events

        /// <summary>
        /// Computes the total duration of the player's sequence of feedbacks
        /// </summary>
        void ComputeCachedTotalDuration()
        {
            float total = 0f;
            if (FeedbacksList == null)
            {
                _cachedTotalDuration = InitialDelay;
                return;
            }
            
            foreach (Feedback feedback in this.FeedbacksList)
            {
                feedback.ComputeTotalDuration();
                if (feedback != null && feedback.Active)
                {
                    if (total < feedback.TotalDuration)
                    {
                        total = feedback.TotalDuration;
                    }
                }
            }
            
            _cachedTotalDuration = InitialDelay + total;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Applies this feedback's time multiplier to a duration (in seconds)
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public float ApplyTimeMultiplier(float duration)
        {
            return duration * Mathf.Clamp(DurationMultiplier, _smallValue, float.MaxValue) * _randomDurationMultiplier;
        }
        
        bool CanFeedbackPlay(Feedback feedback)
        {
            if (feedback == null)
            {
                return false;
            }

            if (feedback.Timing == null)
            {
                return false;
            }
            
            return true;
        }

        #endregion
    }
}