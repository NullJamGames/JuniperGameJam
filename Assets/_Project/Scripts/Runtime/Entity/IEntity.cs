using UnityEngine;

namespace NJG.Runtime.Entity
{
    public interface IEntity
    {
        public Vector3 Position { get; }
        public bool IsInvincible { get; }
        
        public void SetInvincible(bool isInvincible);
        public void SetPosition(Vector3 worldPosition);
        public void ApplyModifier(BaseModifierSO modifierData);
        public void SetAccelerationMultiplier(float accelerationMultiplier);
        public void SetMaxSpeedMultiplier(float maxSpeedMultiplier);
    }
}