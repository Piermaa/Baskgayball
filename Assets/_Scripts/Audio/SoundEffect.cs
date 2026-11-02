using System;
using UnityEngine;

namespace BaskgayBall.Audio
{
    [Serializable]
    public class SoundEffect
    {
        [SerializeField] private string soundName;
        [Tooltip("Audio clip(s) for this sound. If multiple clips are set, one is selected at random.")]
        [SerializeField] private AudioClip[] clips = Array.Empty<AudioClip>();

        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;

        [Range(0.1f, 3f)]
        [SerializeField] private float minPitch = 0.95f;

        [Range(0.1f, 3f)]
        [SerializeField] private float maxPitch = 1.05f;

        [Tooltip("Minimum time (seconds) required before this sound can be played again to prevent spamming.")]
        [SerializeField] private float cooldown = 0.05f;

        private float _lastPlayedTime = -999f;

        public string SoundName => soundName;
        public float Volume => volume;
        public float MinPitch => minPitch;
        public float MaxPitch => maxPitch;
        public float Cooldown => cooldown;
        public AudioClip[] Clips => clips;

        public SoundEffect()
        {
            volume = 1f;
            minPitch = 0.95f;
            maxPitch = 1.05f;
            cooldown = 0.05f;
        }

        public SoundEffect(string name, float defaultVolume = 1f, float defaultMinPitch = 0.95f, float defaultMaxPitch = 1.05f, float defaultCooldown = 0.05f)
        {
            soundName = name;
            volume = defaultVolume;
            minPitch = defaultMinPitch;
            maxPitch = defaultMaxPitch;
            cooldown = defaultCooldown;
        }

        /// <summary>
        /// Checks if this sound has valid clips and its cooldown period has passed.
        /// </summary>
        public bool CanPlay()
        {
            if (clips == null || clips.Length == 0) return false;
            return (Time.unscaledTime - _lastPlayedTime) >= cooldown;
        }

        /// <summary>
        /// Returns a random clip from the assigned clips array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0) return null;
            if (clips.Length == 1) return clips[0];
            return clips[UnityEngine.Random.Range(0, clips.Length)];
        }

        /// <summary>
        /// Returns a randomized pitch within [minPitch, maxPitch].
        /// </summary>
        public float GetRandomPitch()
        {
            if (Mathf.Approximately(minPitch, maxPitch)) return minPitch;
            return UnityEngine.Random.Range(minPitch, maxPitch);
        }

        /// <summary>
        /// Registers that this sound was played just now to reset the cooldown timer.
        /// </summary>
        public void RegisterPlay()
        {
            _lastPlayedTime = Time.unscaledTime;
        }
    }
}

