using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class MainMenuPanel : MenuPanel
    {
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _newRunButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _leaderboardButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _upgradeShopButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _optionsButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _creditsButton;
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _quitButton;

        private Action _onNewRun;
        private Action _onLeaderboard;
        private Action _onUpgradeShop;
        private Action _onOptions;
        private Action _onCredits;
        
        public void Initialize(Action onNewRun, Action onLeaderboard, Action onUpgradeShop, Action onOptions, Action onCredits)
        {
            _onNewRun = onNewRun;
            _onLeaderboard = onLeaderboard;
            _onUpgradeShop = onUpgradeShop;
            _onOptions = onOptions;
            _onCredits = onCredits;
            
            _newRunButton.onClick.AddListener(OnButton_NewRun);
            _leaderboardButton.onClick.AddListener(OnButton_Leaderboard);
            _upgradeShopButton.onClick.AddListener(OnButton_UpgradeShop);
            _optionsButton.onClick.AddListener(OnButton_Options);
            _creditsButton.onClick.AddListener(OnButton_Credits);
            _quitButton.onClick.AddListener(OnButton_Quit);
        }
        
        private void OnButton_NewRun()
        {
            _onNewRun?.Invoke();
        }
        
        private void OnButton_Leaderboard()
        {
            _onLeaderboard?.Invoke();
        }
        
        private void OnButton_UpgradeShop()
        {
            _onUpgradeShop?.Invoke();
        }
        
        private void OnButton_Options()
        {
            _onOptions?.Invoke();
        }
        
        private void OnButton_Credits()
        {
            _onCredits?.Invoke();
        }
        
        private void OnButton_Quit()
        {
            Application.Quit();
        }
    }
}