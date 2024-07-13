using System;
using UnityEngine;

namespace HAWStudio.Common
{
    /// <summary>
    /// Add this class to a GameObject to have it play a background music when instantiated.
    /// </summary>
    public class BackgroundMusic : MonoBehaviour
    {
        [Tooltip("the audio clip to use as background music")]
        public AudioClip SoundClip;
        [Tooltip("whether or not the music should loop")]
        public bool Loop = true;
        [Tooltip("the ID to create this background music with")]
        public int ID = 255;

        void Start()
        {
            SoundManagerPlayOptions options = SoundManagerPlayOptions.Default;
            options.ID = this.ID;
            options.Loop = this.Loop;
            options.Location = Vector3.zero;
            options.SoundManagerTrack = SoundManager.SoundManagerTracks.Music;

            SoundManagerSoundPlayEvent.Trigger(this.SoundClip, options);
        }
    }
}