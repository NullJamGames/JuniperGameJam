using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public abstract class BaseModifierSO : ScriptableObject
    {
        [field: SerializeField]
        public bool DisplayVisual { get; private set; } = true;
        [field: SerializeField, ShowIf(nameof(DisplayVisual))]
        public Color Color { get; private set; } = Color.white;
        [field: SerializeField, ShowIf(nameof(DisplayVisual))]
        public Sprite Sprite { get; private set; }
        
        public abstract void OnApplyModifier(IEntity entity);
        public abstract void OnRemoveModifier(IEntity entity);
    }
}