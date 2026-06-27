using System;
using NJG.Runtime.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class OptionsPanel : MenuPanel
    {
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _backButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Slider _masterVolumeSlider;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Slider _musicVolumeSlider;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Slider _sfxVolumeSlider;

        private void Start()
        {
            _backButton.onClick.AddListener(OnButton_Back);
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            
            _masterVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.VolumeType.Master);
            _musicVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.VolumeType.Music);
            _sfxVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.VolumeType.Effects);
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
            _masterVolumeSlider.onValueChanged.RemoveAllListeners();
            _musicVolumeSlider.onValueChanged.RemoveAllListeners();
            _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        private void OnButton_Back()
        {
            OnHide();
        }

        private void OnMasterVolumeChanged(float value)
        {
            AudioManager.Instance.ChangeVolume(AudioManager.VolumeType.Master, value);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance.ChangeVolume(AudioManager.VolumeType.Music, value);
        }
        
        private void OnSfxVolumeChanged(float value)
        {
            AudioManager.Instance.ChangeVolume(AudioManager.VolumeType.Effects, value);
        }
    }
}