using System;
using UnityEngine;
using UnityEngine.Events;

namespace HAWStudio.Common
{
    public struct FeedbacksEvent
    {
        public delegate void Delegate(FeedbackPlayer source, EventTypes types);
        static event Delegate OnEvent;
        
        public enum EventTypes
        {
            Play, Pause, Resume, Revert, Complete,
        }
        
        public static void Trigger(FeedbackPlayer source, EventTypes type)
        {
            OnEvent?.Invoke(source, type);
        }
    }
    
    /// <summary>
    /// A subclass of Feedbacks, contains UnityEvents that can be played, 
    /// </summary>
    [Serializable]
    public class FeedbacksEvents
    {
        [Tooltip("whether or not this Feedbacks should fire FeedbacksEvents")] 
        public bool TriggerFeedbacksEvents = false;
        [Tooltip("whether or not this Feedbacks should fire Unity Events")] 
        public bool TriggerUnityEvents = true;
        /// This event will fire every time this MMFeedbacks gets played
        [Tooltip("This event will fire every time this MMFeedbacks gets played")]
        public UnityEvent OnPlay;
        /// This event will fire every time this MMFeedbacks starts a holding pause
        [Tooltip("This event will fire every time this MMFeedbacks starts a holding pause")]
        public UnityEvent OnPause;
        /// This event will fire every time this MMFeedbacks resumes after a holding pause
        [Tooltip("This event will fire every time this MMFeedbacks resumes after a holding pause")]
        public UnityEvent OnResume;
        /// This event will fire every time this MMFeedbacks reverts its play direction
        [Tooltip("This event will fire every time this MMFeedbacks reverts its play direction")]
        public UnityEvent OnRevert;
        /// This event will fire every time this MMFeedbacks plays its last MMFeedback
        [Tooltip("This event will fire every time this MMFeedbacks plays its last MMFeedback")]
        public UnityEvent OnComplete;
        
        public bool OnPlayIsNull { get; protected set; }
        public bool OnPauseIsNull { get; protected set; }
        public bool OnResumeIsNull { get; protected set; }
        public bool OnRevertIsNull { get; protected set; }
        public bool OnCompleteIsNull { get; protected set; }

        public void Initialize()
        {
            OnPlayIsNull = OnPlay == null;
            OnPauseIsNull = OnPause == null;
            OnResumeIsNull = OnResume == null;
            OnRevertIsNull = OnRevert == null;
            OnCompleteIsNull = OnComplete == null;
        }

        public void TriggerOnPlay(FeedbackPlayer source)
        {
            if (!OnPlayIsNull && this.TriggerUnityEvents)
            {
                this.OnPlay.Invoke();
            }

            if (this.TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Play);
            }
        }

        public void TriggerOnComplete(FeedbackPlayer source)
        {
            if (!OnCompleteIsNull && this.TriggerUnityEvents)
            {
                this.OnComplete.Invoke();
            }

            if (this.TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Complete);
            }
        }
    }
}