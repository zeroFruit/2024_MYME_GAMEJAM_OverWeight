using System;
using UnityEngine;

namespace HAWStudio.Common
{
    [Serializable]
    public class FeedbackTiming
    {
        [Header("Exceptions")]
        [Tooltip("whether to count this feedback in the parent MMFeedbacks(Player) total duration or not")]
        public bool ContributeToTotalDuration = true;
        
        [Header("Delays")]
        [Tooltip("the initial delay to apply before playing the delay (in seconds)")]
        public float InitialDelay = 0f;
        [Tooltip("the cooldown duration mandatory between two plays")]
        public float CooldownDuration = 0f;
        
        [Header("Stop")]
        /// if this is true, this feedback will interrupt itself when Stop is called on its parent MMFeedbacks, otherwise it'll keep running
        [Tooltip("if this is true, this feedback will interrupt itself when Stop is called on its parent MMFeedbacks, otherwise it'll keep running")]
        public bool InterruptsOnStop = true;

        
        [Header("Sequence")]
        // [Tooltip("A Sequence to use to play these feedbacks on")]
        // public Sequence Sequence;
        [Tooltip("The Sequence's TrackID to consider")]
        public int TrackID = 0;
        
        [Header("Repeat")]
        /// the repeat mode, whether the feedback should be played once, multiple times, or forever
        [Tooltip("the repeat mode, whether the feedback should be played once, multiple times, or forever")]
        public int NumberOfRepeats = 0;
        /// if this is true, the feedback will be repeated forever
        [Tooltip("if this is true, the feedback will be repeated forever")]
        public bool RepeatForever = false;
        /// the delay (in seconds) between two firings of this feedback. This doesn't include the duration of the feedback. 
        [Tooltip("the delay (in seconds) between two firings of this feedback. This doesn't include the duration of the feedback.")]
        public float DelayBetweenRepeats = 1f;

        
        /// from any class, you can set UseScriptDrivenTimescale:true, from there, instead of looking at Time.time, Time.deltaTime (or their unscaled equivalents), this feedback will compute time based on the values you feed them via ScriptDrivenDeltaTime and ScriptDrivenTime
        public bool UseScriptDrivenTimescale { get; set; }
        /// the value this feedback should use for delta time
        public float ScriptDrivenDeltaTime { get; set; }
        /// the value this feedback should use for time
        public float ScriptDrivenTime { get; set; }
    }
}