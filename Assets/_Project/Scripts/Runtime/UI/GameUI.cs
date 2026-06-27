using System;
using System.Collections.Generic;
using NJG.Runtime.Entity;
using NJG.Runtime.Events;
using NJG.Runtime.Input;
using NJG.Runtime.Managers;
using NJG.Runtime.Online;
using NJG.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NJG.Runtime.UI
{
    public class GameUI : MonoBehaviour
    {
        [FoldoutGroup("Panels"), SerializeField]
        private MainMenuPanel _mainMenuPanel;
        [FoldoutGroup("Panels"), SerializeField]
        private LeaderboardPanel _leaderboardPanel;
        [FoldoutGroup("Panels"), SerializeField]
        private UpgradePanel _upgradePanel;
        [FoldoutGroup("Panels"), SerializeField]
        private OptionsPanel _optionsPanel;
        [FoldoutGroup("Panels"), SerializeField]
        private CreditsPanel _creditsPanel;
        [FoldoutGroup("Panels"), SerializeField]
        private PlayerHUD _playerHUD;
        [FoldoutGroup("Panels"), SerializeField]
        private PausePanel _pausePanel;

        private IInputProvider _input;
        
        private void Awake()
        {
            _leaderboardPanel.Initialize(OnButton_NewRun, OnButton_UpgradeShop, OnButton_MainMenu);
            _mainMenuPanel.Initialize(OnButton_NewRun, OnButton_Leaderboard, OnButton_UpgradeShop, OnButton_Options, OnButton_Credits);
            _pausePanel.Init(OnButton_Options);
        }

        private void OnEnable()
        {
            EventBus.StartListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StartListening<GameOverEvent>(OnGameOver);
            EventBus.StartListening<NewRunEvent>(OnNewRun);
            
            // Modifer Events
            EventBus.StartListening<AddedModifierEvent>(OnAddedModifier);
            EventBus.StartListening<RemovedModifierEvent>(OnRemovedModifier);
        }

        private void Start()
        {
            _input = GameManager.Instance.InputProvider;
            
            HideAllPanels();
            _mainMenuPanel.OnShow();
        }

        private void Update()
        {
            if (_input.WasPausePressed() && GameManager.Instance.TryPauseGame())
            {
                _pausePanel.OnShow();
            }
        }

        private void OnDisable()
        {
            EventBus.StopListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StopListening<GameOverEvent>(OnGameOver);
            EventBus.StopListening<NewRunEvent>(OnNewRun);
            
            // Modifer Events
            EventBus.StopListening<AddedModifierEvent>(OnAddedModifier);
            EventBus.StopListening<RemovedModifierEvent>(OnRemovedModifier);
        }

        private void OnAddedModifier(AddedModifierEvent e)
        {
            _playerHUD.AddModifier(e.Modifier);
        }
        
        private void OnRemovedModifier(RemovedModifierEvent e)
        {
            _playerHUD.RemoveModifier(e.Modifier);
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            _playerHUD.SetScore(e.Score);
        }

        private void OnGameOver(GameOverEvent e)
        {
            HideAllPanels();
            _leaderboardPanel.OnShow();
            _leaderboardPanel.ShowGameOverText(e.Score, e.HighScore);
        }

        private void OnNewRun(NewRunEvent e)
        {
            HideAllPanels();
            _playerHUD.OnShow();
        }
        
        private void OnButton_NewRun()
        {
            GameManager.Instance.NewRun();
        }
        
        private void OnButton_MainMenu()
        {
            HideAllPanels();
            _mainMenuPanel.OnShow();
        }

        private void OnButton_Leaderboard()
        {
            _mainMenuPanel.OnHide();
            _leaderboardPanel.OnShow();
        }

        private void OnButton_UpgradeShop()
        {
            _upgradePanel.OnShow();
        }

        private void OnButton_Options()
        {
            _optionsPanel.OnShow();
        }
        
        private void OnButton_Credits()
        {
            _creditsPanel.OnShow();
        }
        
        private void HideAllPanels()
        {
            _pausePanel.OnHide();
            _playerHUD.OnHide();
            _mainMenuPanel.OnHide();
            _leaderboardPanel.OnHide();
            _upgradePanel.OnHide();
            _optionsPanel.OnHide();
            _creditsPanel.OnHide();
        }
    }
}