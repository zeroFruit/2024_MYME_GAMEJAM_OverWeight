using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace HAWStudio.Common
{
    [Serializable]
    public abstract class Feedback : MonoBehaviour
    {
        #region Properties

        [Header("Feedback Settings")]
        [Tooltip("whether or not this feedback is active")]
        public bool Active = true;

        [HideInInspector] public int UniqueID;
        
        [Tooltip("the name of this feedback to display in the inspector")]
        public string Label = "MMFeedback";

        [Tooltip(
            "whether to broadcast this feedback's message using an int or a scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what. " +
            "MMChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable")]
        public ChannelModes ChannelMode = ChannelModes.Int;
        [Tooltip("the ID of the channel on which this feedback will communicate")]
        [EnumCondition("ChannelMode", (int)ChannelModes.Int)]
        public int Channel = 0;
        
        [Tooltip(
            "the MMChannel definition asset to use to broadcast this feedback. The shaker will have to reference that same MMChannel definition to receive events - to create a MMChannel, " +
            "right click anywhere in your project (usually in a Data folder) and go MoreMountains > MMChannel, then name it with some unique name")]
        [EnumCondition("ChannelMode", (int)ChannelModes.Channel)]
        public Channel ChannelDefinition = null;

        [Tooltip("a number of timing-related values (delay, repeat, etc)")]
        public FeedbackTiming Timing;
        
        [HideInInspector] public FeedbackPlayer Owner;
        [HideInInspector] public bool DebugActive = false;
        
		/// returns true if this feedback is in cooldown at this time (and thus can't play), false otherwise
		public virtual bool InCooldown => (Timing.CooldownDuration > 0f) &&
		                                  (FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration);

		/// if this is true, this feedback is currently playing
		public virtual bool IsPlaying { get; set; }
		
		public float FeedbackDeltaTime => Time.deltaTime;
		
		/// <summary>
		/// Returns the t value at which to evaluate a curve at the end of this feedback's play time
		/// </summary>
		protected float FinalNormalizedTime => 1f;

		/// the time (or unscaled time) based on the selected Timing settings
		public float FeedbackTime
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					return (float)EditorApplication.timeSinceStartup;
				}
#endif
				if (this.Timing.UseScriptDrivenTimescale)
				{
					return this.Timing.ScriptDrivenTime;
				}
				
				// if (Owner.ForceTimescaleMode)
				// {
				// 	if (Owner.ForcedTimescaleMode == TimescaleModes.Scaled)
				// 	{
				// 		return Time.time;
				// 	}
				// 	else
				// 	{
				// 		return Time.unscaledTime;
				// 	}
				// }
				
				return Time.unscaledTime; 
			}
		}
		
		// the perceived duration of the feedback, to be used to display its progress bar, meant to be overridden with meaningful data by each feedback
		public virtual float FeedbackDuration
		{
			get { return 0f; }
			set { }
		}
		
		/// <summary>
		/// The total duration of this feedback :
		/// total = initial delay + duration * (number of repeats + delay between repeats)  
		/// </summary>
		public virtual float TotalDuration
		{
			get
			{
				return _totalDuration;
			}
		}
		
		public ChannelData ChannelData(int channel) => this._channelData.Set(ChannelModes.Int, channel, null);
		
		protected float _lastPlayTimestamp = -1f;
		protected int _playsLeft;
		protected bool _initialized = false;
		protected Coroutine _playCoroutine;
		protected Coroutine _infinitePlayCoroutine;
		protected Coroutine _sequenceCoroutine;
		protected Coroutine _repeatedPlayCoroutine;
		protected ChannelData _channelData;
		protected float _totalDuration = 0f;
		protected int _indexInOwnerFeedbackList = 0;
		
        #endregion

        #region Initialization

        /// <summary>
        /// Runs at Awake, lets you preinitialize your custom feedback before Initialization
        /// </summary>
        public virtual void PreInitialization(FeedbackPlayer owner, int index)
        {
	        this._channelData = new ChannelData(this.ChannelMode, this.Channel, this.ChannelDefinition);
        }

        /// <summary>
        /// Typically runs on Start, Initializes the feedback and its timing related variables
        /// </summary>
        public virtual void Initialization(FeedbackPlayer owner, int index)
        {
	        if (this.Timing == null)
	        {
		        this.Timing = new FeedbackTiming();
	        }   
	        
	        SetIndexInFeedbacksList(index);
	        this._lastPlayTimestamp = -1f;
	        this._initialized = true;
	        this.Owner = owner;
	        // this._playsLeft = this.Timing
	        this._channelData = new ChannelData(this.ChannelMode, this.Channel, this.ChannelDefinition);
	        
	        SetInitialDelay(Timing.InitialDelay);
	        CustomInitialization(owner);
        }
        
        /// <summary>
        /// Lets you specify at what index this feedback is in the list - use carefully (or don't use at all)
        /// </summary>
        /// <param name="index"></param>
        public virtual void SetIndexInFeedbacksList(int index)
        {
	        _indexInOwnerFeedbackList = index;
        }

        #endregion

        #region Automation

        

        #endregion


        #region Play

        /// <summary>
        /// Plays the feedback
        /// </summary>
        public void Play(Vector3 position, float feedbacksIntensity = 1.0f)
        {
	        if (!Active)
	        {
		        return;
	        }

	        if (!_initialized)
	        {
		        string feedbackName = this.ToString().Replace("MoreMountains.Feedbacks.", "");
		        Debug.LogWarning("The " + feedbackName +
		                         " feedback on " + Owner.gameObject.name +
		                         " is being played without having been initialized. Always call the Initialization() method first. This can be done manually, or on Start or Awake (automatically on Start is the default). If you're auto playing your feedback on Start or on Enable, initialize on Awake (which runs before Start and Enable). You can change that setting on your MMF Player, unfold the Settings foldout at the top, and change the Initialization Mode.",
			        Owner.gameObject);
	        }

	        // we check the cooldown
	        if (InCooldown)
	        {
		        return;
	        }

	        if (Timing.InitialDelay > 0f)
	        {
		        _playCoroutine = Owner.StartCoroutine(PlayCoroutine(position, feedbacksIntensity));
	        }
	        else
	        {
		        RegularPlay(position, feedbacksIntensity);
		        _lastPlayTimestamp = FeedbackTime;
	        }
        }

        IEnumerator PlayCoroutine(Vector3 position, float feedbacksIntensity = 1.0f)
        {
	        yield return CoroutineHelper.WaitFor(this.Timing.InitialDelay);
	        this._lastPlayTimestamp = FeedbackTime;
	        this.RegularPlay(position, feedbacksIntensity);
        }

        void RegularPlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
	        CustomPlayFeedback(position, feedbacksIntensity);
        }
        
        /// <summary>
        /// A coroutine used to play this feedback on a sequence
        /// </summary>
        IEnumerator SequencePlayRoutine(Vector3 position, float feedbacksIntensity = 1.0f)
        {
	        yield return null;
	        float timeStartedAt = FeedbackTime;
	        float lastFrame = FeedbackTime;
            
	        // TODO: Timing.Quantized ?

	        // while (FeedbackTime - timeStartedAt < this.Timing.Sequence.Length)
	        // {
		       //  foreach (SequenceNote note in this.Timing.Sequence.OriginalSequence.Line)
		       //  {
			      //   if (note.ID == this.Timing.TrackID && note.Timestamp >= lastFrame && note.Timestamp <= FeedbackTime - timeStartedAt)
			      //   {
				     //    CustomPlayFeedback(position, feedbacksIntensity);
			      //   }
		       //  }
	        //
		       //  lastFrame = FeedbackTime - timeStartedAt;
		       //  yield return null;
	        // }
        }
        
        public void ResetFeedback()
        {
	        this.CustomReset();
        }

        #endregion
        
        #region Controls

        /// <summary>
        /// Stops all feedbacks from playing. Will stop repeating feedbacks, and call custom stop implementations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Stop(Vector3 position, float feedbacksIntensity = 1.0f)
        {
	        if (_playCoroutine != null)
	        {
		        Owner.StopCoroutine(_playCoroutine);
	        }
	        
	        if (_infinitePlayCoroutine != null)
	        {
		        Owner.StopCoroutine(_infinitePlayCoroutine);
	        }

	        if (_repeatedPlayCoroutine != null)
	        {
		        Owner.StopCoroutine(_repeatedPlayCoroutine);
	        }

	        if (_sequenceCoroutine != null)
	        {
		        Owner.StopCoroutine(_sequenceCoroutine);
	        }
	        
	        _lastPlayTimestamp = -1f;
	        _playsLeft = Timing.NumberOfRepeats + 1;
	        if (Timing.InterruptsOnStop)
	        {
		        CustomStopFeedback(position, feedbacksIntensity);
	        }
        }

        #endregion

        #region Sequence

        // /// <summary>
        // /// Use this method to change this feedback's sequence at runtime
        // /// </summary>
        // /// <param name="newSequence"></param>
        // public virtual void SetSequence(Sequence newSequence)
        // {
	       //  Timing.Sequence = newSequence;
	       //  if (Timing.Sequence != null)
	       //  {
		      //   for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
		      //   {
			     //    if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
			     //    {
				    //     _sequenceTrackID = i;
			     //    }
		      //   }
	       //  }
        // }


        #endregion

        #region Time

        /// <summary>
        /// Use this method to specify a new initial delay at runtime
        /// </summary>
        /// <param name="delay"></param>
        public virtual void SetInitialDelay(float delay)
        {
	        Timing.InitialDelay = delay;
        }

        /// <summary>
        /// Computes the total duration of this feedback
        /// </summary>
        public virtual void ComputeTotalDuration()
        {
	        if ((Timing != null) && (!Timing.ContributeToTotalDuration))
	        {
		        _totalDuration = 0f;
		        return;
	        }
	        
	        float totalTime = 0f;

	        if (Timing == null)
	        {
		        _totalDuration = 0f;
		        return;
	        }

	        if (Timing.InitialDelay != 0)
	        {
		        totalTime += ApplyTimeMultiplier(Timing.InitialDelay);
	        }
	        
	        totalTime += FeedbackDuration;

	        this._totalDuration = totalTime;
        }

        /// <summary>
        /// Applies the host MMFeedbacks' time multiplier to this feedback
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected virtual float ApplyTimeMultiplier(float duration)
        {
	        if (Owner == null)
	        {
		        return 0f;
	        }
	        
	        return Owner.ApplyTimeMultiplier(duration);
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// This method describes all custom initialization processes the feedback requires, in addition to the main Initialization method
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(FeedbackPlayer owner) { }
        
        /// <summary>
        /// This method describes what happens when the feedback gets played
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f);

        /// <summary>
        /// This method describes what happens when the feedback gets stopped
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f) { }
        
        /// <summary>
        /// This method describes what happens when the feedback gets reset
        /// </summary>
        protected virtual void CustomReset() { }

        #endregion
    }
}