using System;
using System.Collections.Generic;
using System.Linq;
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
        
        [FoldoutGroup("Modifiers UI"), SerializeField]
        private GameObject _buffIndicatorContainer;
        [FoldoutGroup("Modifiers UI"), SerializeField]
        private BuffIndicatorVisual _buffIndicatorPrefab;

        [FoldoutGroup("Game Over"), SerializeField]
        private GameObject _gameOverPanel;
        [FoldoutGroup("Game Over"), SerializeField]
        private TextMeshProUGUI _gameOverScoreText;
        [FoldoutGroup("Game Over"), SerializeField]
        private Button _restartButton;
        
        private List<BuffIndicatorVisual> _buffIndicators = new ();

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(OnButton_Restart);
            
            EventBus.StartListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StartListening<GameOverEvent>(OnGameOver);
            
            // Modifer Events
            EventBus.StartListening<AddedModifierEvent>(OnAddedModifier);
            EventBus.StartListening<UpdatedModifierEvent>(OnUpdateModifier);
            EventBus.StartListening<RemovedModifierEvent>(OnRemovedModifier);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(OnButton_Restart);
            
            EventBus.StopListening<ScoreChangedEvent>(OnScoreChanged);
            EventBus.StopListening<GameOverEvent>(OnGameOver);
            
            // Modifer Events
            EventBus.StopListening<AddedModifierEvent>(OnAddedModifier);
            EventBus.StopListening<UpdatedModifierEvent>(OnUpdateModifier);
            EventBus.StopListening<RemovedModifierEvent>(OnRemovedModifier);
        }

        private void OnAddedModifier(AddedModifierEvent e)
        {
            BuffIndicatorVisual indicator = Instantiate(_buffIndicatorPrefab, _buffIndicatorContainer.transform);
            indicator.SetUp(e.Modifier);
            _buffIndicators.Add(indicator);
        }
        
        private void OnUpdateModifier(UpdatedModifierEvent e)
        {
            BuffIndicatorVisual indicator = _buffIndicators.FirstOrDefault(indicator => indicator.ModifierData == e.Modifier);
            if (indicator == null)
            {
                Log.E($"No indicator found for modifier {e.Modifier.name}");
                return;
            }
            
            indicator.UpdateDuration(e.RemainingDuration);
        }
        
        private void OnRemovedModifier(RemovedModifierEvent e)
        {
            for (int i = 0; i < _buffIndicators.Count; i++)
            {
                if (_buffIndicators[i].ModifierData == e.Modifier)
                {
                    Destroy(_buffIndicators[i].gameObject);
                    _buffIndicators.RemoveAt(i);
                    break;
                }
            }
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