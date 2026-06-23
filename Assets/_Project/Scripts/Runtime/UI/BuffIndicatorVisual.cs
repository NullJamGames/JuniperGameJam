using System;
using NJG.Runtime.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class BuffIndicatorVisual : MonoBehaviour
    {
        private Image _image;
        
        public BaseModifierSO ModifierData { get; private set; }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetUp(BaseModifierSO modifierData)
        {
            ModifierData = modifierData;
            _image.color = modifierData.Color;
            _image.fillAmount = 1f;
        }

        public void UpdateDuration(float remainingDuration)
        {
            _image.fillAmount = remainingDuration / ModifierData.Duration;
        }
    }
}