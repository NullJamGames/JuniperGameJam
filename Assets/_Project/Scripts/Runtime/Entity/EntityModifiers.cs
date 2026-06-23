using System.Collections.Generic;
using System.Linq;
using NJG.Runtime.Events;
using NJG.Utilities;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EntityModifiers
    {
        private IEntity _entity;
        
        public List<(BaseModifierSO Modifier, float RemainingTime)> TemporaryModifiers { get; private set; } = new();
        public List<BaseModifierSO> PermanentModifiers { get; private set; } = new();
        
        public EntityModifiers(IEntity entity)
        {
            _entity = entity;
        }

        public void ProcessModifiers()
        {
            if (TemporaryModifiers.Count == 0) return;

            for (int i = TemporaryModifiers.Count - 1; i >= 0; i--)
            {
                (BaseModifierSO modifier, float remaining) = TemporaryModifiers[i];
                remaining -= Time.deltaTime;

                if (remaining <= 0f)
                {
                    TemporaryModifiers.RemoveAt(i);
                    modifier.OnRemoveModifier(_entity);
                    EventBus.TriggerEvent(new RemovedModifierEvent(modifier));
                }
                else
                {
                    TemporaryModifiers[i] = (modifier, remaining);
                    EventBus.TriggerEvent(new UpdatedModifierEvent(modifier, remaining));
                }
            }
        }
        
        public void AddModifier(BaseModifierSO modifier)
        {
            if (PermanentModifiers.Contains(modifier))
                return;
            
            if (TemporaryModifiers.Exists(m => m.Modifier == modifier))
            {
                // Reset the duration if the modifier is already applied
                for (int i = 0; i < TemporaryModifiers.Count; i++)
                {
                    if (TemporaryModifiers[i].Modifier == modifier)
                    {
                        TemporaryModifiers[i] = (modifier, modifier.Duration);
                        break;
                    }
                }
                return;
            }
            
            if (modifier.IsTemporary)
                TemporaryModifiers.Add((modifier, modifier.Duration));
            else
                PermanentModifiers.Add(modifier);
                
            modifier.OnApplyModifier(_entity);
                
            EventBus.TriggerEvent(new AddedModifierEvent(modifier));
        }
        
        public void RemoveModifier(BaseModifierSO modifier)
        {
            if (PermanentModifiers.Contains(modifier))
                PermanentModifiers.Remove(modifier);
            else
                TemporaryModifiers.RemoveAll(m => m.Modifier == modifier);
        }
    }
}