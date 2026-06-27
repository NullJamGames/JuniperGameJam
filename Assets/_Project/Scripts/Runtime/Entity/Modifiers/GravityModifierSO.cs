using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "GravityModifier", menuName = "NJG/Modifiers/Gravity")]
    public class GravityModifierSO : BaseModifierSO
    {
        [field: SerializeField]
        public float GravityMultiplier { get; private set; } = 0.7f;

        public override void OnApplyModifier(IEntity entity)
        {
            entity.Stats.ApplyGravityMultiplier(EntityStats.Operation.Multiply, GravityMultiplier);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.Stats.ApplyGravityMultiplier(EntityStats.Operation.Divide, GravityMultiplier);
        }
    }
}