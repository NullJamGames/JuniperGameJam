using UnityEngine;

namespace NJG.Utilities
{
    public interface IPooledGO
    {
        public void SetOwnerPool(GameObjectPool ownerPool);
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation);
        public void OnGetFromPool();
        public void OnReturnToPool();
        public void ReturnToPool();
    }
}