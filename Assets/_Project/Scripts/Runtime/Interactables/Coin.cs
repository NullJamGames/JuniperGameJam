using NJG.Runtime.Entity;
using NJG.Runtime.Managers;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Coin : MonoBehaviour, IPickupable
    {
        [SerializeField]
        private int _coinsAmount = 1;
        [SerializeField]
        private AudioClip _pickupSound;
        
        public void Pickup(IEntity entity)
        {
            entity.PlaySound(_pickupSound);
            GameManager.Instance.AddPlayerCoins(_coinsAmount);
            Destroy(gameObject);
        }
    }
}