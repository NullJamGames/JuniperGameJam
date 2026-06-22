using System.Collections.Generic;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EntityModifiers
    {
        public List<(InvincibleModifierSO Modifier, float RemainingTime)> TemporaryModifiers { get; private set; } = new();
        public List<InvincibleModifierSO> PermanentModifiers { get; private set; } = new();

        public void ProcessModifiers()
        {
            if (TemporaryModifiers.Count == 0) return;

            for (int i = TemporaryModifiers.Count - 1; i >= 0; i--)
            {
                (InvincibleModifierSO modifier, float remaining) = TemporaryModifiers[i];
                remaining -= Time.deltaTime;

                if (remaining <= 0f)
                {
                    TemporaryModifiers.RemoveAt(i);
                    // TODO: notify that modifier expired if needed
                }
                else
                {
                    TemporaryModifiers[i] = (modifier, remaining);
                }
            }
        }

        public void AddModifier(InvincibleModifierSO modifierData)
        {
            if (modifierData.IsTemporary)
                TemporaryModifiers.Add((modifierData, modifierData.Duration));
            else
                PermanentModifiers.Add(modifierData);
            
            // TODO: properly notify that a modifier was added
        }
    }
}