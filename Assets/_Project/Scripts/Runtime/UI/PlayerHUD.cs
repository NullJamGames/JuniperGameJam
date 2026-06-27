using System.Collections.Generic;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NJG.Runtime.UI
{
    public class PlayerHUD : MenuPanel
    {
        [FoldoutGroup("Score"), SerializeField]
        private TextMeshProUGUI _scoreText;
        
        [FoldoutGroup("Modifiers"), SerializeField]
        private GameObject _buffIndicatorContainer;
        [FoldoutGroup("Modifiers"), SerializeField]
        private GameObject _heartIndicatorContainer;
        [FoldoutGroup("Modifiers"), SerializeField]
        private BuffIndicatorVisual _buffIndicatorPrefab;
        
        private List<BuffIndicatorVisual> _buffIndicators = new ();
        
        public void SetScore(int score)
        {
            _scoreText.SetText($"Score: {score}");
        }

        public void AddModifier(Modifier modifier)
        {
            if (!modifier.ModifierData.DisplayVisual)
                return;
            
            Transform containerTransform = modifier.ModifierData is LifeModifierSO ? _heartIndicatorContainer.transform : _buffIndicatorContainer.transform;
            BuffIndicatorVisual indicator = Instantiate(_buffIndicatorPrefab, containerTransform);
            indicator.SetUp(modifier);
            _buffIndicators.Add(indicator);
        }
        
        public void RemoveModifier(Modifier modifier)
        {
            for (int i = 0; i < _buffIndicators.Count; i++)
            {
                if (_buffIndicators[i].Modifier == modifier)
                {
                    Destroy(_buffIndicators[i].gameObject);
                    _buffIndicators.RemoveAt(i);
                    break;
                }
            }
        }
    }
}