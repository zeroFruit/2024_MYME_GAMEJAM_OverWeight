using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
    /// A class to save sound settings (music on or off, sfx on or off)
    /// </summary>
    [CreateAssetMenu(fileName = "SoundManagerSettings", menuName = "HAWStudio/Audio/SoundManagerSettings")]
    public class SoundManagerSettingsSO : ScriptableObject
    {
        /// the possible ways to manage a track
        public enum MMSoundManagerTracks { Sfx, Music, UI, Master, Other}
        
        [Tooltip("the audio mixer to use when playing sounds")]
        public AudioMixer TargetAudioMixer;
        /// the master group
        [Tooltip("the master group")]
        public AudioMixerGroup MasterAudioMixerGroup;
        /// the group on which to play all music sounds
        [Tooltip("the group on which to play all music sounds")]
        public AudioMixerGroup MusicAudioMixerGroup;
        /// the group on which to play all sound effects
        [Tooltip("the group on which to play all sound effects")]
        public AudioMixerGroup SfxAudioMixerGroup;
        /// the group on which to play all UI sounds
        [Tooltip("the group on which to play all UI sounds")]
        public AudioMixerGroup UIAudioMixerGroup;
        /// the multiplier to apply when converting normalized volume values to audio mixer values
        [Tooltip("the multiplier to apply when converting normalized volume values to audio mixer values")]
        public float MixerValuesMultiplier = 20;
        
        [Header("Settings Unfold")]
        [Tooltip("the full settings for this MMSoundManager")]
        public SoundManagerSettings Settings;
        
        protected const string _saveFolderName = "SoundManager/";
        protected const string _saveFileName = "sound.json";

        #region Save And Load

        public virtual void SaveSoundSettings()
        {
            // SaveLoadManager.Save(this.Settings, _saveFileName, _saveFolderName);
        }

        public virtual void LoadSoundSettings()
        {
            // if (!this.Settings.OverrideMixerSettings)
            // {
            //     return;
            // }
            //
            // SoundManagerSettings settings =
            //     (SoundManagerSettings)SaveLoadManager.Load(typeof(SoundManagerSettings), _saveFileName, _saveFolderName);
            // if (settings != null)
            // {
            //     this.Settings = settings;
            //     this.ApplyTrackVolumes();
            // }
            //
            // SoundManagerEvent.Trigger(SoundManagerEventTypes.SettingsLoaded);
        }

        #endregion

        #region Volume

        /// <summary>
        /// applies volume to all tracks and saves if needed
        /// </summary>
        protected virtual void ApplyTrackVolumes()
        {
            if (!this.Settings.OverrideMixerSettings)
            {
                return;
            }

            this.TargetAudioMixer.SetFloat(this.Settings.MasterVolumeParameter,
                NormalizedToMixerVolume(this.Settings.MasterVolume));

            if (!this.Settings.MasterOn)
            {
                this.TargetAudioMixer.SetFloat(this.Settings.MasterVolumeParameter, -80f);
            }

            if (this.Settings.AutoSave)
            {
                this.SaveSoundSettings();
            }
        }

        /// <summary>
        /// Converts a normalized volume to the mixer group db scale
        /// </summary>
        /// <param name="normalizedVolume"></param>
        /// <returns></returns>
        public virtual float NormalizedToMixerVolume(float normalizedVolume)
        {
            return Mathf.Log10(normalizedVolume) * this.MixerValuesMultiplier;
        }

        #endregion
    }