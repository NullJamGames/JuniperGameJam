using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "SpeedModifier", menuName = "NJG/Modifiers/Speed")]
    public class SpeedModifierSO : BaseModifierSO
    {
        [field: SerializeField]
        public float AccelerationMultiplier { get; private set; } = 1.5f;
        [field: SerializeField]
        public float MaxSpeedMultiplier { get; private set; } = 1.5f;
        
        public override void OnApplyModifier(IEntity entity)
        {
            entity.Stats.ApplyAccelerationMultiplier(EntityStats.Operation.Multiply, AccelerationMultiplier);
            entity.Stats.ApplyMaxSpeedMultiplier(EntityStats.Operation.Multiply, MaxSpeedMultiplier);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.Stats.ApplyAccelerationMultiplier(EntityStats.Operation.Divide, AccelerationMultiplier);
            entity.Stats.ApplyMaxSpeedMultiplier(EntityStats.Operation.Divide, MaxSpeedMultiplier);
        }
    }
}