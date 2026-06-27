using System;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EntityAudio : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _hitClip;
        [SerializeField]
        private AudioClip[] _crashClips;
        [SerializeField]
        private AudioClip _landClip;
        
        [Header("Wheel Audio"), SerializeField]
        private AudioSource _wheelAudioSource;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySFX(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        public void PlayHitSFX()
        {
            _audioSource.PlayOneShot(_hitClip);
        }

        public void PlayCrashSFX()
        {
            _audioSource.PlayOneShot(_crashClips[UnityEngine.Random.Range(0, _crashClips.Length)]);
        }

        public void PlayLandSFX()
        {
            _audioSource.PlayOneShot(_landClip);
        }

        public void ToggleWheelSound(bool isMoving)
        {
            if (isMoving)
            {
                if (!_wheelAudioSource.isPlaying)
                {
                    _wheelAudioSource.Play();
                }
            }
            else
            {
                if (_wheelAudioSource.isPlaying)
                {
                    _wheelAudioSource.Stop();
                }
            }
        }
    }
}