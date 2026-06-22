using System;
using NJG.Runtime.Interactables;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EntityBreakerTrigger : MonoBehaviour
    {
        private IEntity _entity;

        private void Awake()
        {
            _entity = GetComponentInParent<IEntity>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_entity.IsInvincible)
                return;
            
            if (other.TryGetComponent(out IBreakable breakable))
            {
                breakable.Break();
            }
        }
    }
}