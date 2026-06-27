using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "LifeModifier", menuName = "NJG/Modifiers/Life")]
    public class LifeModifierSO : BaseModifierSO
    {
        public override void OnApplyModifier(IEntity entity)
        {
            entity.Stats.AddExtraLives(1);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.Stats.TryRemoveExtraLives(1);
        }
    }
}