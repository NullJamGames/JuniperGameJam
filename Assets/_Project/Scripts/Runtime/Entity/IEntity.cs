using UnityEngine;

namespace NJG.Runtime.Entity
{
    public interface IEntity
    {
        public Vector3 Position { get; }
        public bool IsInvincible { get; }
        public EntityStats Stats { get; }
        
        public void SetInvincible(bool isInvincible);
        public void SetPosition(Vector3 worldPosition);
        public void ApplyModifier(Modifier modifier);
        public void PlaySound(AudioClip clip);
    }
}