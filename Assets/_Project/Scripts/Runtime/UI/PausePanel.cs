using System;
using NJG.Runtime.Events;
using NJG.Runtime.Managers;
using NJG.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class PausePanel : MenuPanel
    {
        [SerializeField]
        private Button _resumeButton;
        [SerializeField]
        private Button _optionsButton;
        [SerializeField]
        private Button _endRunButton;

        private Action _onOptionsPressed;
        
        protected override void Awake()
        {
            base.Awake();
            
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        public void Init(Action onOptionsPressed)
        {
            _onOptionsPressed = onOptionsPressed;
            _optionsButton.onClick.AddListener(OnButton_Options);
            _endRunButton.onClick.AddListener(OnButton_EndRun);
        }

        private void OnResumeButtonClicked()
        {
            OnHide();
            GameManager.Instance.TryResumeGame();
        }
        
        private void OnButton_Options()
        {
            _onOptionsPressed?.Invoke();
        }
        
        private void OnButton_EndRun()
        {
            OnHide();
            EventBus.TriggerEvent(new EndRunRequestEvent());
        }
    }
}