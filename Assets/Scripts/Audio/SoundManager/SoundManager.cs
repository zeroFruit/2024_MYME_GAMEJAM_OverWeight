using System.Collections;
using System.Collections.Generic;
using HAWStudio.Common;
using SSR.OverWeight;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
    /// A simple yet powerful sound manager, that will let you play sounds with an event based approach and performance in mind.
    /// 
    /// Features :
    /// 
    /// - Play/stop/pause/resume/free sounds
    /// - Full control : loop, volume, pitch, pan, spatial blend, bypasses, priority, reverb, doppler level, spread, rolloff mode, distance
    /// - 2D & 3D spatial support
    /// - Built-in pooling, automatically recycle a set of audio sources for maximum performance
    /// - Built in audio mixer and groups, with ready-made tracks (Master, Music, SFX, UI), and options to play on more groups if needed
    /// - Stop/pause/resume/free entire tracks
    /// - Stop/pause/resume/free all sounds at once
    /// - Mute / set volume entire tracks
    /// - Save and load settings, with auto save / auto load mechanics built-in
    /// - Fade in/out sounds
    /// - Fade in/out tracks
    /// - Solo mode : play a sound with one or all tracks muted, then unmute them automatically afterwards
    /// - PlayOptions struct
    /// - Option to have sounds persist across scene loads and from scene to scene
    /// - Inspector controls for tracks (volume, mute, unmute, play, pause, stop, resume, free, number of sounds)
    /// - SfxEvents
    /// - SoundManagerEvents : mute track, control track, save, load, reset, stop persistent sounds 
    /// </summary>
    public class SoundManager : Singleton<SoundManager>,
        EventListener<SoundManagerSoundControlEvent>
    {
        /// the possible ways to manage a track
        public enum SoundManagerTracks { Sfx, Music, UI, Master, Other}
        
        [Header("Settings")]
        [Tooltip("the current sound settings ")]
        public SoundManagerSettingsSO Settings;
        [Tooltip("the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once")]
        public int AudioSourcePoolSize = 10;
        [Tooltip("whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.")]
        public bool PoolCanExpand = true;

        SoundManagerAudioPool _pool;
        GameObject _tempAudioSourceGameObject;
        SoundManagerSound _sound;
        List<SoundManagerSound> _sounds;
        Dictionary<AudioSource, Coroutine> _fadeSoundCoroutines;
        AudioSource _tempAudioSource;

        #region Initialization

        protected override void Awake()
        {
            base.Awake();
            this.InitializeSoundManager();
        }

        void InitializeSoundManager()
        {
            if (this._pool == null)
            {
                this._pool = new SoundManagerAudioPool();
            }

            this._sounds = new List<SoundManagerSound>();
            this._pool.FillAudioSourcePool(this.AudioSourcePoolSize, this.transform);
            this._fadeSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        }

        #endregion

        
        #region PlaySound

        AudioSource PlaySound(AudioClip audioClip, SoundManagerPlayOptions options)
        {
            if (this == null)
            {
                return null;
            }

            if (!audioClip)
            {
                return null;
            }

            AudioSource recycleAudioSource;
            // we reuse an audiosource if one is passed in parameters
            AudioSource audioSource = options.RecycleAudioSource;

            if (audioSource == null)
            {
                audioSource = this._pool.GetAvailableAudioSource(this.PoolCanExpand, this.transform);
                if (audioSource != null && !options.Loop)
                {
                    recycleAudioSource = audioSource;
                    // we destroy the host after the clip has played (if it not tag for reusability.
                    StartCoroutine(this._pool.AutoDisableAudioSource(audioClip.length / Mathf.Abs(options.Pitch),
                        audioSource, audioClip, options.DoNotAutoRecycleIfNotDonePlaying, options.PlaybackTime,
                        options.PlaybackDuration));
                }
            }

            if (audioSource == null)
            {
                this._tempAudioSourceGameObject = new GameObject("Audio_" + audioClip.name);
                SceneManager.MoveGameObjectToScene(_tempAudioSourceGameObject, this.gameObject.scene);
                audioSource = _tempAudioSourceGameObject.AddComponent<AudioSource>();
            }
            
            // audio source settings
            audioSource.transform.position = options.Location;
            audioSource.clip = audioClip;
            audioSource.pitch = options.Pitch;
            audioSource.loop = options.Loop;
            audioSource.priority = options.Priority;
            audioSource.time = options.PlaybackTime;
            
            // curves
            
            // attaching to target
            
            // track and volume
            if (this.Settings != null)
            {
                audioSource.outputAudioMixerGroup = this.Settings.MasterAudioMixerGroup;
                switch (options.SoundManagerTrack)
                {
                    case SoundManagerTracks.Sfx:
                        audioSource.outputAudioMixerGroup = this.Settings.SfxAudioMixerGroup;
                        break;
                    case SoundManagerTracks.Music:
                        audioSource.outputAudioMixerGroup = this.Settings.MusicAudioMixerGroup;
                        break;
                }
            }

            if (options.AudioGroup)
            {
                audioSource.outputAudioMixerGroup = options.AudioGroup;
            }

            audioSource.volume = options.Volume;
            
            // we start playing the sound
            audioSource.Play();
            
            // we destroy the host after the clip has played if it was a one time audio source
            if (!options.Loop && !options.RecycleAudioSource)
            {
                float destroyDelay = (options.PlaybackDuration > 0) ? options.PlaybackDuration : audioClip.length - options.PlaybackTime;
                Destroy(this._tempAudioSourceGameObject, destroyDelay);
            }
            
            // we fade the sound in if needed
            if (options.Fade)
            {
                FadeSound(audioSource, options.FadeDuration, options.FadeInitialVolume, options.Volume, options.FadeTween);
            }
            
            // we handle soloing
            
            // we prepare for storage
            this._sound.ID = options.ID;
            this._sound.Track = options.SoundManagerTrack;
            this._sound.Source = audioSource;
            this._sound.Persistent = options.Persistent;
            this._sound.PlaybackTime = options.PlaybackTime;
            this._sound.PlaybackDuration = options.PlaybackDuration;
            
            // we check if that audio source is already being tracked in _sounds
            bool alreadyIn = false;
            for (int i = 0; i < this._sounds.Count; i++)
            {
                if (this._sounds[i].Source == audioSource)
                {
                    this._sounds[i] = this._sound;
                    alreadyIn = true;
                }
            }

            if (!alreadyIn)
            {
                this._sounds.Add(this._sound);
            }

            return audioSource;
        }

        #endregion

        #region SoundControls

        /// <summary>
        /// Pauses the specified audiosource
        /// </summary>
        /// <param name="source"></param>
        public virtual void PauseSound(AudioSource source)
        {
            source.Pause();
        }

        /// <summary>
        /// resumes play on the specified audio source
        /// </summary>
        /// <param name="source"></param>
        public virtual void ResumeSound(AudioSource source)
        {
            source.Play();
        }
        
        /// <summary>
        /// Stops the specified audio source
        /// </summary>
        /// <param name="source"></param>
        public virtual void StopSound(AudioSource source)
        {
            source.Stop();
        }
        
        /// <summary>
        /// Frees a specific sound, stopping it and returning it to the pool
        /// </summary>
        /// <param name="source"></param>
        public virtual void FreeSound(AudioSource source)
        {
            source.Stop();
            if (!_pool.FreeSound(source))
            {
                Destroy(source.gameObject);    
            }
        }


        #endregion
        

        #region Fade

        /// <summary>
        /// Fades a target sound towards a final volume over time
        /// </summary>
        void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume, TweenType tweenType)
        {
            Coroutine coroutine =
                StartCoroutine(FadeSoundCoroutine(source, duration, initialVolume, finalVolume, tweenType));
            this._fadeSoundCoroutines[source] = coroutine;
        }

        /// <summary>
        /// Fades an audiosource's volume over time
        /// </summary>
        IEnumerator FadeSoundCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume, TweenType tweenType)
        {
            float startedAt = Time.unscaledTime;
            if (tweenType == null)
            {
                tweenType = new TweenType(Tweens.TweenCurve.EaseInOutQuartic);
            }
            while (Time.unscaledTime - startedAt <= duration)
            {
                float elapsedTime = Time.unscaledTime - startedAt;
                float newVolume = Tweens.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
                source.volume = newVolume;
                yield return null;
            }
            source.volume = finalVolume;
        }

        #endregion

        #region Find

        /// <summary>
        /// Returns an audio source played with the specified ID, if one is found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual AudioSource FindByID(int id)
        {
            foreach (SoundManagerSound sound in this._sounds)
            {
                if (sound.ID == id)
                {
                    return sound.Source;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an audio source played with the specified ID, if one is found
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public virtual AudioSource FindByClip(AudioClip clip)
        {
            foreach (SoundManagerSound sound in this._sounds)
            {
                if (sound.Source.clip == clip)
                {
                    return sound.Source;
                }
            }

            return null;
        }

        #endregion


        #region Events
        
        public void OnEvent(SoundManagerSoundControlEvent e)
        {
            if (e.TargetSource == null)
            {
                this._tempAudioSource = FindByID(e.SoundID);
            }
            else
            {
                this._tempAudioSource = e.TargetSource;
            }

            if (this._tempAudioSource == null)
            {
                return;
            }

            switch (e.EventType)
            {
                case SoundManagerSoundControlEventTypes.Pause:
                    this.PauseSound(this._tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Resume:
                    this.ResumeSound(this._tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Stop:
                    this.StopSound(this._tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Free:
                    this.FreeSound(this._tempAudioSource);
                    break;
            }
        }

        void OnSfxEvent(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f,
            float pitch = 1f, int priority = 128)
        {
            SoundManagerPlayOptions options = SoundManagerPlayOptions.Default;
            options.Location = this.transform.position;
            options.AudioGroup = audioGroup;
            options.Volume = volume;
            options.Pitch = pitch;
            if (priority >= 0)
            {
                options.Priority = Mathf.Min(priority, 256);
            }
            options.SoundManagerTrack = SoundManagerTracks.Sfx;
            options.Loop = false;
            
            PlaySound(clipToPlay, options);
        }

        AudioSource OnSoundManagerSoundPlayEvent(AudioClip clip, SoundManagerPlayOptions options)
        {
            return PlaySound(clip, options);
        }

        void OnEnable()
        {
            SfxEvent.Register(this.OnSfxEvent);
            SoundManagerSoundPlayEvent.Register(this.OnSoundManagerSoundPlayEvent);
            this.StartListeningEvent<SoundManagerSoundControlEvent>();
        }

        void OnDisable()
        {
            SfxEvent.Unregister(this.OnSfxEvent);
            SoundManagerSoundPlayEvent.Unregister(this.OnSoundManagerSoundPlayEvent);
            this.StopListeningEvent<SoundManagerSoundControlEvent>();
        }

        #endregion
    }