using UnityEngine;

namespace NJG.Utilities
{
    public sealed class PooledGameObject : MonoBehaviour, IPooledGO
    {
        private GameObjectPool _ownerPool;

        public void SetOwnerPool(GameObjectPool ownerPool)
        {
            _ownerPool = ownerPool;
        }
        
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }
        
        public void OnGetFromPool()
        {
            
        }

        public void OnReturnToPool()
        {
            
        }

        public void ReturnToPool()
        {
            if (_ownerPool == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{name} has no owner pool, destroying instead.", this);
#endif
                Destroy(gameObject);
                return;
            }

            _ownerPool.Return(gameObject);
        }
    }
}