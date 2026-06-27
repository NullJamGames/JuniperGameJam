using System;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class Modifier
    {
        public BaseModifierSO ModifierData { get; private set; }
        public bool IsPermanent { get; private set; }
        public float Duration { get; private set; }
        public float RemainingDuration { get; private set; }
        public Color Color { get; private set; }

        public event Action OnModifierDurationUpdated;
        
        public Modifier(BaseModifierSO modifierData, bool isPermanent = false, float duration = 0, Color color = default)
        {
            ModifierData = modifierData;
            IsPermanent = isPermanent;
            Duration = duration;
            RemainingDuration = duration;
            Color = color;
        }
        
        public void Tick(float deltaTime)
        {
            if (IsPermanent)
                return;

            RemainingDuration -= deltaTime;
            OnModifierDurationUpdated?.Invoke();
        }

        public void ResetDuration()
        {
            if (IsPermanent)
                return;

            RemainingDuration = Duration;
            OnModifierDurationUpdated?.Invoke();
        }
    }
}