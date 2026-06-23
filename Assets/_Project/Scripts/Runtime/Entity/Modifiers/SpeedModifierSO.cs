using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "SpeedModifier", menuName = "NJG/Modifiers/Speed")]
    public class SpeedModifierSO : BaseModifierSO
    {
        [field: SerializeField]
        public float AccelerationMultiplier { get; private set; } = 1.25f;
        [field: SerializeField]
        public float MaxSpeedMultiplier { get; private set; } = 1.25f;
        
        public override void OnApplyModifier(IEntity entity)
        {
            entity.SetAccelerationMultiplier(AccelerationMultiplier);
            entity.SetMaxSpeedMultiplier(MaxSpeedMultiplier);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.SetAccelerationMultiplier(1f);
            entity.SetMaxSpeedMultiplier(1f);
        }
    }
}