using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace HAWStudio.Common
{
    /// <summary>
    /// This feedback lets you play the specified AudioClip, either via event (you'll need something in your scene to catch a MMSfxEvent,
    /// for example a MMSoundManager), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play).
    /// For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness),
    /// random pitch, and an optional AudioMixerGroup.
    /// </summary>
    public class FeedbackSound : Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        
        /// <summary>
        /// The possible methods to play the sound with. 
        /// Event : sends a MMSfxEvent, you'll need a class to catch this event and play the sound
        /// Cached : creates and stores an audiosource to play the sound with, parented to the owner
        /// OnDemand : creates an audiosource and destroys it everytime you want to play the sound
        /// </summary>
        public enum PlayMethods { Event, Cached }
        
        [Header("Sound")]
        [Tooltip("the sound clip to play")]
        public AudioClip Sfx;
        [Tooltip("an array to pick a random sfx from")]
        public AudioClip[] RandomSfx;
        
        [Header("Play Method")]
        [Tooltip("the play method to use when playing the sound (event, cached or on demand)")]
        public PlayMethods PlayMethod = PlayMethods.Event;
        [Tooltip("if this is true, calling Stop on this feedback will also stop the sound from playing further")]
        public bool StopSoundOnFeedbackStop = true;
        
        [Header("Sound Properties")]
        [Header("Volume")]
        [Tooltip("the minimum volume to play the sound at")]
        [Range(0f,2f)]
        public float MinVolume = 1f;
        [Tooltip("the maximum volume to play the sound at")]
        [Range(0f,2f)]
        public float MaxVolume = 1f;

        [Header("Pitch")]
        [Tooltip("the minimum pitch to play the sound at")]
        [Range(-3f,3f)]
        public float MinPitch = 1f;
        [Tooltip("the maximum pitch to play the sound at")]
        [Range(-3f,3f)]
        public float MaxPitch = 1f;
        
        [Header("Mixer")]
        [Tooltip("the audiomixer to play the sound with (optional)")]
        public AudioMixerGroup SfxAudioMixerGroup;
        [Tooltip("the audiosource priority, to be specified if needed between 0 (highest) and 256")] 
        public int Priority = 128;
        
        // [Header("3D Sound Settings")]

        public override float FeedbackDuration
        {
            get
            {
                return this.GetDuration();
            }
        }
        
        float _duration;
        AudioClip _randomClip;
        AudioSource _cachedAudioSource;
        AudioSource _tempAudioSource;
        AudioSource _audioSource;

        protected override void CustomInitialization(FeedbackPlayer owner)
        {
            base.CustomInitialization(owner);
            if (this.PlayMethod == PlayMethods.Cached)
            {
                _cachedAudioSource = CreateAudioSource(owner.gameObject, "CachedFeedbackAudioSource");
            }
        }

        AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
        {
            // we create a temp game object to host our audio source
            GameObject tempAudioHost = new GameObject(audioSourceName);
            SceneManager.MoveGameObjectToScene(tempAudioHost.gameObject, this.Owner.gameObject.scene);
            
            // we set the temp audio's position
            tempAudioHost.transform.position = owner.transform.position;
            tempAudioHost.transform.SetParent(owner.transform);
            
            // we add an audio source to that host
            this._tempAudioSource = tempAudioHost.AddComponent<AudioSource>();
            this._tempAudioSource.playOnAwake = false;
            return this._tempAudioSource;
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            float intensityMultiplier = feedbacksIntensity;

            if (this.Sfx != null)
            {
                this._duration = this.Sfx.length;
                PlaySound(this.Sfx, position, intensityMultiplier);
            }

            if (this.RandomSfx.Length > 0)
            {
                this._randomClip = this.RandomSfx[Random.Range(0, this.RandomSfx.Length)];
                if (this._randomClip != null)
                {
                    this._duration = this._randomClip.length;
                    PlaySound(this._randomClip, position, intensityMultiplier);
                }
            }
        }
        
        protected virtual float GetDuration()
        {
            if (Sfx != null)
            {
                return Sfx.length;
            }

            float longest = 0f;
            if (this.RandomSfx != null && this.RandomSfx.Length > 0)
            {
                foreach (AudioClip clip in this.RandomSfx)
                {
                    if (clip != null && clip.length > longest)
                    {
                        longest = clip.length;
                    }
                }

                return longest;
            }

            return 0f;
        }

        /// <summary>
        /// Plays a sound differently based on the selected play method
        /// </summary>
        void PlaySound(AudioClip sfx, Vector3 position, float intensity)
        {
            float volume = Random.Range(MinVolume, MaxVolume);
            float pitch = Random.Range(MinPitch, MaxPitch);
            int timeSamples = 0;

            switch (this.PlayMethod)
            {
                case PlayMethods.Event:
                    SfxEvent.Trigger(sfx, this.SfxAudioMixerGroup, volume, pitch, this.Priority);
                    break;
                case PlayMethods.Cached:
                    // we set that audio source clip to the one in paramaters
                    PlayAudioSource(_cachedAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup, Priority);
                    break;
            }
            
        }

        /// <summary>
        /// Plays the audio source with the specified volume and pitch
        /// </summary>
        void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch, int timeSamples, AudioMixerGroup audioMixerGroup = null, int priority = 128)
        {
            this._audioSource = audioSource;
            // we set that audio source clip to the one in parameters
            audioSource.clip = sfx;
            audioSource.timeSamples = timeSamples;
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.priority = priority;
            // we set spatial settings
            
            // we set our loop setting
            audioSource.loop = false;
            if (audioMixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = audioMixerGroup;
            }
            
            // we start playing the sound
            audioSource.Play();
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!this.Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            
            if (StopSoundOnFeedbackStop && (_audioSource != null))
            {
                _audioSource.Stop();
            }
        }
    }
}