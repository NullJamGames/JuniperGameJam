using UnityEngine;

namespace NJG.Runtime.Entity
{
    [CreateAssetMenu(fileName = "InvincibleModifier", menuName = "NJG/Modifiers/Invincible")]
    public class InvincibleModifierSO : BaseModifierSO
    {
        public override void OnApplyModifier(IEntity entity)
        {
            entity.SetInvincible(true);
        }

        public override void OnRemoveModifier(IEntity entity)
        {
            entity.SetInvincible(false);
        }
    }
}