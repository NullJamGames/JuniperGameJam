using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public abstract class BaseModifierSO : ScriptableObject
    {
        [field: SerializeField]
        public bool IsTemporary { get; private set; } = true;
        [field: SerializeField, ShowIf(nameof(IsTemporary))]
        public float Duration { get; private set; } = 5f;
        [field: SerializeField, ShowIf(nameof(IsTemporary))]
        public Color Color { get; private set; } = Color.white;
        
        public abstract void OnApplyModifier(IEntity entity);
        public abstract void OnRemoveModifier(IEntity entity);
    }
}