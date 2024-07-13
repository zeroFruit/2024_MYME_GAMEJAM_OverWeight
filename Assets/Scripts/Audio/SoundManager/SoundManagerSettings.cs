using System;
using UnityEngine;

/// <summary>
    /// This class stores SoundManager settings and lets you tweak them from the SoundManagerSettingsSO's inspector
    /// </summary>
    [Serializable]
    public class SoundManagerSettings
    {
        public const float _minimalVolume = 0.0001f;
        public const float _maxVolume = 10f;
        public const float _defaultVolume = 1f;
        
        [Header("Audio Mixer Control")] 
		/// whether or not the settings described below should override the ones defined in the AudioMixer 
		[Tooltip("whether or not the settings described below should override the ones defined in the AudioMixer")]
		public bool OverrideMixerSettings = true;

		[Header("Audio Mixer Exposed Parameters")]
		[Tooltip("the name of the exposed MasterVolume parameter in the AudioMixer")]
		public string MasterVolumeParameter = "MasterVolume";
		[Tooltip("the name of the exposed MusicVolume parameter in the AudioMixer")]
		public string MusicVolumeParameter = "MusicVolume";
		[Tooltip("the name of the exposed SfxVolume parameter in the AudioMixer")]
		public string SfxVolumeParameter = "SfxVolume";
		[Tooltip("the name of the exposed UIVolume parameter in the AudioMixer")]
		public string UIVolumeParameter = "UIVolume";
        
		[Header("Master")]
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the master volume")]
		[ReadOnly]
		public float MasterVolume = _defaultVolume;
		[Tooltip("whether the master track is active at the moment or not")]
		[ReadOnly] 
		public bool MasterOn = true;
		[Tooltip("the volume of the master track before it was muted")]
		[ReadOnly] 
		public float MutedMasterVolume;

		[Header("Music")]
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the music volume")]
		[ReadOnly]
		public float MusicVolume = _defaultVolume; 
		[Tooltip("whether the music track is active at the moment or not")]
		[ReadOnly] 
		public bool MusicOn = true;
		[Tooltip("the volume of the music track before it was muted")]
		[ReadOnly] 
		public float MutedMusicVolume;
        
		[Header("Sound Effects")]
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the sound fx volume")]
		[ReadOnly]
		public float SfxVolume = _defaultVolume;
		[Tooltip("whether the SFX track is active at the moment or not")]
		[ReadOnly] 
		public bool SfxOn = true;
		[Tooltip("the volume of the SFX track before it was muted")]
		[ReadOnly] 
		public float MutedSfxVolume;
        
		[Header("UI")]
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the UI sounds volume")]
		[ReadOnly]
		public float UIVolume = _defaultVolume;
		[Tooltip("whether the UI track is active at the moment or not")]
		[ReadOnly] 
		public bool UIOn = true;
		[Tooltip("the volume of the UI track before it was muted")]
		[ReadOnly] 
		public float MutedUIVolume;
        
		[Header("Save & Load")]
		[Tooltip("whether or not the MMSoundManager should automatically load settings when starting")]
		public bool AutoLoad = true;
		[Tooltip("whether or not each change in the settings should be automaticall saved. If not, you'll have to call a save MMSoundManager event for settings to be saved.")]
		public bool AutoSave = false;
    }