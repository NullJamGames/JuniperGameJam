using System;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class PowerUp : MonoBehaviour, IPickupable
    {
        [SerializeField]
        private BaseModifierSO[] _modifiers;

        [BoxGroup("Settings"), SerializeField]
        private bool _isPermanent;
        [BoxGroup("Settings"), SerializeField, HideIf(nameof(_isPermanent))]
        private float _duration;
        
        [BoxGroup("Visual"), SerializeField]
        private float _rotationSpeed;
        
        [BoxGroup("Audio"), SerializeField]
        private AudioClip _pickupSound;

        private void Update()
        {
            transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
        }

        public void Pickup(IEntity entity)
        {
            foreach (BaseModifierSO modifier in _modifiers)
            {
                Modifier modifierInstance = new (modifier, _isPermanent, _duration, modifier.Color);
                entity.ApplyModifier(modifierInstance);
                entity.PlaySound(_pickupSound);
            }
            
            Destroy(gameObject);
        }
    }
}