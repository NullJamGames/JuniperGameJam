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
        private EntityStats _stats;
        
        public List<Modifier> Modifiers { get; private set; } = new();
        
        public EntityModifiers(IEntity entity, EntityStats stats)
        {
            _entity = entity;
            _stats = stats;
        }

        public void ProcessModifiers()
        {
            if (Modifiers.Count == 0) return;

            for (int i = Modifiers.Count - 1; i >= 0; i--)
            {
                Modifier modifier = Modifiers[i];
                if (modifier.IsPermanent)
                    continue;
                
                modifier.Tick(Time.deltaTime);

                if (modifier.RemainingDuration <= 0f)
                {
                    Modifiers.RemoveAt(i);
                    modifier.ModifierData.OnRemoveModifier(_entity);
                    EventBus.TriggerEvent(new RemovedModifierEvent(modifier));
                }
            }
        }
        
        public void AddModifier(Modifier modifier)
        {
            Modifier existingModifier = Modifiers.FirstOrDefault(m => m.ModifierData == modifier.ModifierData);
            if (existingModifier != null)
            {
                existingModifier.ResetDuration();
                return;
            }
            
            Modifiers.Add(modifier);
            modifier.ModifierData.OnApplyModifier(_entity);
            EventBus.TriggerEvent(new AddedModifierEvent(modifier));
        }
        
        public void RemoveModifier(Modifier modifier)
        {
            if (!Modifiers.Contains(modifier))
                return;
            
            Modifiers.Remove(modifier);
            modifier.ModifierData.OnRemoveModifier(_entity);
            EventBus.TriggerEvent(new RemovedModifierEvent(modifier));
        }
        
        public bool TryRemoveModifierByType<T>() where T : BaseModifierSO
        {
            Modifier modifierToRemove = Modifiers.FirstOrDefault(m => m.ModifierData is T);
            if (modifierToRemove != null)
            {
                RemoveModifier(modifierToRemove);
                return true;
            }
            return false;
        }
        
        public void RemoveAllModifiers()
        {
            foreach (Modifier modifier in Modifiers)
            {
                modifier.ModifierData.OnRemoveModifier(_entity);
                EventBus.TriggerEvent(new RemovedModifierEvent(modifier));
            }
            Modifiers.Clear();
        }
    }
}