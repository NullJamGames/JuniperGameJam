using System;
using NJG.Runtime.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class BuffIndicatorVisual : MonoBehaviour
    {
        private Image _image;
        
        public Modifier Modifier { get; private set; }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetUp(Modifier modifier)
        {
            Modifier = modifier;
            _image.color = modifier.Color;
            if (modifier.ModifierData.Sprite != null) _image.sprite = modifier.ModifierData.Sprite;
            _image.fillAmount = 1f;
            
            Modifier.OnModifierDurationUpdated += UpdateVisualDuration;
        }

        private void OnDestroy()
        {
            if (Modifier != null)
                Modifier.OnModifierDurationUpdated -= UpdateVisualDuration;
        }

        public void UpdateVisualDuration()
        {
            _image.fillAmount = Modifier.RemainingDuration / Modifier.Duration;
        }
    }
}