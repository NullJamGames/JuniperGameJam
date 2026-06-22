using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "InvincibleModifier", menuName = "NJG/Modifiers/Invincible")]
    public class InvincibleModifierSO : ScriptableObject
    {
        [field: SerializeField]
        public bool IsTemporary { get; private set; } = true;
        [field: SerializeField, ShowIf(nameof(IsTemporary))]
        public float Duration { get; private set; } = 5f;
    }
}