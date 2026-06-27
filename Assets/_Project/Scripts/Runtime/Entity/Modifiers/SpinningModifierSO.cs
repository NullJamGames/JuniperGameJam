using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "SpinningModifier", menuName = "NJG/Modifiers/Spinning")]
    public class SpinningModifierSO : BaseModifierSO
    {
        [field: SerializeField]
        public float SpinTorqueMultiplier { get; private set; } = 2f;
        [field: SerializeField]
        public float MaxAngularSpeedMultiplier { get; private set; } = 2f;
        [field: SerializeField]
        public float LateralInfluenceMultiplier { get; private set; } = 2f;
        
        public override void OnApplyModifier(IEntity entity)
        {
            entity.Stats.ApplySpinTorqueMultiplier(EntityStats.Operation.Multiply, SpinTorqueMultiplier);
            entity.Stats.ApplyMaxAngularSpeedMultiplier(EntityStats.Operation.Multiply, MaxAngularSpeedMultiplier);
            entity.Stats.ApplyLateralInfluenceMultiplier(EntityStats.Operation.Multiply, LateralInfluenceMultiplier);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.Stats.ApplySpinTorqueMultiplier(EntityStats.Operation.Divide, SpinTorqueMultiplier);
            entity.Stats.ApplyMaxAngularSpeedMultiplier(EntityStats.Operation.Divide, MaxAngularSpeedMultiplier);
            entity.Stats.ApplyLateralInfluenceMultiplier(EntityStats.Operation.Divide, LateralInfluenceMultiplier);
        }
    }
}