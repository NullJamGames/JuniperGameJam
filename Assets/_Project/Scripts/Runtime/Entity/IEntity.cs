using UnityEngine;

namespace NJG.Runtime.Entity
{
    public interface IEntity
    {
        public Vector3 Position { get; }
        public bool IsInvincible { get; }
        public void SetPosition(Vector3 worldPosition);
        public void ApplyModifier(InvincibleModifierSO modifierData);
    }
}