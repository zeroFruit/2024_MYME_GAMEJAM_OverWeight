using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
    /// This class manages an object pool of audiosources
    /// </summary>
    [Serializable]
    public class SoundManagerAudioPool
    {
        List<AudioSource> _pool;

        /// <summary>
        /// Fills the pool with ready-to-use audiosources
        /// </summary>
        public void FillAudioSourcePool(int poolSize, Transform parent)
        {
            if (this._pool == null)
            {
                this._pool = new List<AudioSource>();
            }
            
            if ((poolSize <= 0) || (_pool.Count >= poolSize))
            {
                return;
            }

            foreach (AudioSource source in _pool)
            {
                UnityEngine.Object.Destroy(source.gameObject);
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject audioHost = new GameObject("AudioSourcePool_" + i);
                SceneManager.MoveGameObjectToScene(audioHost.gameObject, parent.gameObject.scene);
                AudioSource audioSource = audioHost.AddComponent<AudioSource>();
                // FollowTarget followTarget = audioHost.AddComponent<FollowTarget>();
                // followTarget.enabled = false;
                // followTarget.DisableSelfOnSetActiveFalse = true;
                audioHost.transform.SetParent(parent);
                audioHost.SetActive(false);
                this._pool.Add(audioSource);
            }
        }

        /// <summary>
        /// Disables an audio source after it's done playing
        /// </summary>
        public IEnumerator AutoDisableAudioSource(float duration, AudioSource source, AudioClip clip,
            bool doNotAutoRecycleIfNotDonePlaying, float playbackTime, float playbackDuration)
        {
            float initialWait = (playbackDuration > 0) ? playbackDuration : duration;
            yield return CoroutineHelper.WaitFor(initialWait);
            if (source.clip != clip)
            {
                yield break;
            }

            if (doNotAutoRecycleIfNotDonePlaying)
            {
                float maxTime = (playbackDuration > 0) ? playbackTime + playbackDuration : source.clip.length;
                while (source.time < maxTime)
                {
                    yield return null;
                }
            }
            source.gameObject.SetActive(false);
        }

        /// <summary>
        /// Pulls an available audio source from the pool
        /// </summary>
        public AudioSource GetAvailableAudioSource(bool poolCanExpand, Transform parent)
        {
            foreach (AudioSource source in _pool)
            {
                if (!source.gameObject.activeInHierarchy)
                {
                    source.gameObject.SetActive(true);
                    return source;
                }
            }

            if (!poolCanExpand)
            {
                return null;
            }
            
            GameObject audioHost = new GameObject("AudioSourcePool_" + this._pool.Count);
            SceneManager.MoveGameObjectToScene(audioHost.gameObject, parent.gameObject.scene);
            AudioSource audioSource = audioHost.AddComponent<AudioSource>();
            audioHost.transform.SetParent(parent);
            audioHost.SetActive(false);
            this._pool.Add(audioSource);
            
            return audioSource;
        }

        /// <summary>
        /// Stops an audiosource and returns it to the pool
        /// </summary>
        public virtual bool FreeSound(AudioSource sourceToStop)
        {
            foreach (AudioSource source in this._pool)
            {
                if (source == sourceToStop)
                {
                    source.Stop();
                    source.gameObject.SetActive(false);
                    return true;
                }
            }

            return false;
        }
    }