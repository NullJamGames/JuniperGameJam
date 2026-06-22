using System;
using NJG.Runtime.Events;
using NJG.Runtime.Managers;
using NJG.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class GameUI : MonoBehaviour
    {
        [FoldoutGroup("Score UI"), SerializeField]
        private TextMeshProUGUI _scoreText;

        [FoldoutGroup("Game Over"), SerializeField]
        private GameObject _gameOverPanel;
        [FoldoutGroup("Game Over"), SerializeField]
        private TextMeshProUGUI _gameOverScoreText;
        [FoldoutGroup("Game Over"), SerializeField]
        private Button _restartButton;

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(OnButton_Restart);
            
            EventBus.StartListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StartListening<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(OnButton_Restart);
            
            EventBus.StopListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StopListening<GameOverEvent>(OnGameOver);
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            _scoreText.SetText($"Score: {e.Score}");
        }

        private void OnGameOver(GameOverEvent e)
        {
            _gameOverPanel.SetActive(true);
            _gameOverScoreText.SetText($"Final Score: <color=green>{e.Score}</color>");
        }

        private void OnButton_Restart()
        {
            GameManager.Instance.Restart();
        }
    }
}