using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
    /// A class used to store options for SoundManager play
    /// </summary>
    [Serializable]
    public struct SoundManagerPlayOptions
    {
        /// the track on which to play the sound
        public SoundManager.SoundManagerTracks SoundManagerTrack;
        /// the location at which to position the sound
        public Vector3 Location;
        /// whether or not the sound should loop
        public bool Loop;
        /// the volume at which to play the sound
        public float Volume;
        /// the ID of the sound, useful to find that sound again later
        public int ID;
        /// whether or not to fade the sound when playing it
        public bool Fade;
        /// the initial volume of the sound, before the fade
        public float FadeInitialVolume;
        /// the duration of the fade, in seconds
        public float FadeDuration;
        /// the tween to use when fading the sound
        public TweenType FadeTween;
        /// whether or not the sound should persist over scene transitions
        public bool Persistent;
        /// an AudioSource to use if you don't want to pick one from the pool
        public AudioSource RecycleAudioSource;
        /// an audiogroup to use if you don't want to play on any of the preset tracks
        public AudioMixerGroup AudioGroup;
        /// The pitch of the audio source.
        public float Pitch;
        /// The time (in seconds) at which to start playing the sound
        public float PlaybackTime;
        /// The time (in seconds after which to stop playing the sound
        public float PlaybackDuration;
        /// Sets the priority of the AudioSource.
        public int Priority;
        /// Whether or not the source should be auto recycled if not done playing
        public bool DoNotAutoRecycleIfNotDonePlaying;
        /// Within the Min distance the AudioSource will cease to grow louder in volume.
        public float MinDistance;
        /// (Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.
        public float MaxDistance;
        /// a Transform this sound can 'attach' to and follow it along as it plays
        public Transform AttachToTransform;

        public static SoundManagerPlayOptions Default
        {
            get
            {
                SoundManagerPlayOptions defaultOptions = new SoundManagerPlayOptions();
                defaultOptions.SoundManagerTrack = SoundManager.SoundManagerTracks.Sfx;
                defaultOptions.Location = Vector3.zero;
                defaultOptions.Loop = false;
                defaultOptions.Volume = 1.0f;
                defaultOptions.ID = 0;
                defaultOptions.Fade = false;
                defaultOptions.FadeInitialVolume = 0f;
                defaultOptions.FadeDuration = 1f;
                defaultOptions.FadeTween = null;
                defaultOptions.Persistent = false;
                defaultOptions.RecycleAudioSource = null;
                defaultOptions.AudioGroup = null;
                defaultOptions.Pitch = 1f;
                defaultOptions.Priority = 128;
                defaultOptions.DoNotAutoRecycleIfNotDonePlaying = false;
                defaultOptions.MinDistance = 1f;
                defaultOptions.MaxDistance = 500f;
                return defaultOptions;
            }
        }
    }