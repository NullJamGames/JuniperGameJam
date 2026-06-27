using System;
using System.Collections;
using NJG.Runtime.Events;
using NJG.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace NJG.Runtime.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        public enum VolumeType
        {
            Master,
            Music,
            Effects
        }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Music Clips")]
        [SerializeField] private AudioClip _mainMenuMusic;
        [SerializeField] private AudioClip[] _levelMusicClips;

        [Header("Fade Settings")]
        [SerializeField] private float _mainMenuFadeInDuration = 2f;
        [SerializeField] private float _mainMenuFadeOutDuration = 2f;
        
        [Header("Volume")]
        [SerializeField] private float _masterInitialVolume = 0.5f;
        [SerializeField] private float _musicInitialVolume = 0.75f;
        [SerializeField] private float _effectsInitialVolume = 0.75f;

        private Coroutine _musicRoutine;

        private const string MASTER_PARAM = "MasterVolume";
        private const string MUSIC_PARAM = "MusicVolume";
        private const string EFFECTS_PARAM = "SFXVolume";

        private void OnEnable()
        {
            EventBus.StartListening<NewRunEvent>(OnNewRun);
            EventBus.StartListening<GameOverEvent>(OnRunEnded);
        }

        private void Start()
        {
            ChangeVolume(VolumeType.Master, _masterInitialVolume);
            ChangeVolume(VolumeType.Music, _musicInitialVolume);
            ChangeVolume(VolumeType.Effects, _effectsInitialVolume);
            PlayMainMenuMusic();
        }

        private void OnDisable()
        {
            EventBus.StopListening<NewRunEvent>(OnNewRun);
            EventBus.StopListening<GameOverEvent>(OnRunEnded);
        }

        private void OnNewRun(NewRunEvent e)
        {
            PlayLevelMusic();
        }

        private void OnRunEnded(GameOverEvent e)
        {
            PlayMainMenuMusic();
        }

        public void PlayMainMenuMusic()
        {
            StopMusicRoutine();

            _musicRoutine = StartCoroutine(MainMenuMusicRoutine());
        }

        public void PlayLevelMusic()
        {
            StopMusicRoutine();

            if (_levelMusicClips == null || _levelMusicClips.Length == 0)
                return;

            AudioClip randomClip = _levelMusicClips[Random.Range(0, _levelMusicClips.Length)];

            _musicSource.clip = randomClip;
            _musicSource.loop = true;
            _musicSource.volume = 1f;
            _musicSource.Play();
        }

        private IEnumerator MainMenuMusicRoutine()
        {
            _musicSource.clip = _mainMenuMusic;
            _musicSource.loop = false;

            while (true)
            {
                _musicSource.volume = 0f;
                _musicSource.Play();

                yield return FadeMusicVolume(0f, 1f, _mainMenuFadeInDuration);

                float playTimeBeforeFadeOut =
                    _mainMenuMusic.length - _mainMenuFadeInDuration - _mainMenuFadeOutDuration;

                if (playTimeBeforeFadeOut > 0f)
                    yield return new WaitForSeconds(playTimeBeforeFadeOut);

                yield return FadeMusicVolume(1f, 0f, _mainMenuFadeOutDuration);

                _musicSource.Stop();
            }
        }

        private IEnumerator FadeMusicVolume(float from, float to, float duration)
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float t = timer / duration;
                _musicSource.volume = Mathf.Lerp(from, to, t);

                yield return null;
            }

            _musicSource.volume = to;
        }

        private void StopMusicRoutine()
        {
            if (_musicRoutine != null)
            {
                StopCoroutine(_musicRoutine);
                _musicRoutine = null;
            }

            _musicSource.Stop();
        }

        public void ChangeVolume(VolumeType volumeType, float volume)
        {
            float normalizedVolume = Mathf.Clamp(volume, 0.0001f, 1f);
            float dB = Mathf.Log10(normalizedVolume) * 20f;

            _audioMixer.SetFloat(volumeType switch
            {
                VolumeType.Master => MASTER_PARAM,
                VolumeType.Music => MUSIC_PARAM,
                _ => EFFECTS_PARAM
            }, dB);
        }
        
        public float GetVolume(VolumeType volumeType)
        {
            _audioMixer.GetFloat(volumeType switch
            {
                VolumeType.Master => MASTER_PARAM,
                VolumeType.Music => MUSIC_PARAM,
                _ => EFFECTS_PARAM
            }, out float dB);

            return Mathf.Pow(10f, dB / 20f);
        }
    }
}