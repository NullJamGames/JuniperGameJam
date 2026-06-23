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
        private float _rotationSpeed;

        private void Update()
        {
            transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
        }

        public void Pickup(IEntity entity)
        {
            foreach (BaseModifierSO modifier in _modifiers)
            {
                entity.ApplyModifier(modifier);
            }
            
            Destroy(gameObject);
        }
    }
}