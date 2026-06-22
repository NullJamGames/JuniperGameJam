using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Utilities
{
    public class GameObjectPool : MonoBehaviour
    {
        [FoldoutGroup("Prefab"), SerializeField]
        private GameObject _prefab;
        
        [FoldoutGroup("Sizing"), SerializeField, Min(0)]
        private int _initialPoolSize = 25;
        [SerializeField]
        private bool _isPoolInfinite = false;
        [SerializeField, HideIf(nameof(_isPoolInfinite)), Min(0)]
        private int _maxPoolSize = 100;
        
        [FoldoutGroup("Behavior"), SerializeField]
        private bool _setActiveOnGet = true;
        [FoldoutGroup("Behavior"), SerializeField]
        private bool _setInactiveOnReturn = true;
        [FoldoutGroup("Behavior"), SerializeField]
        private bool _parentToPoolOnReturn = true;

        private Stack<GameObject> _available;
        private HashSet<GameObject> _inUse;
        private int _totalCreated;
        
        public int TotalCreated => _totalCreated;
        public int AvailableCount => _available?.Count ?? 0;
        public int InUseCount => _inUse?.Count ?? 0;

        private void Awake()
        {
            if (_prefab == null)
            {
                Debug.LogError($"{nameof(GameObjectPool)} on {name} has no prefab assigned.", this);
                enabled = false;
                return;
            }
            
            int capacity = Mathf.Max(0, _initialPoolSize);
            
            _available = new Stack<GameObject>(capacity);
            _inUse = new HashSet<GameObject>(capacity);
            
            if (!_isPoolInfinite && _maxPoolSize < _initialPoolSize)
                _maxPoolSize = _initialPoolSize;

            Warm(_initialPoolSize);
        }

        [Button(ButtonSizes.Medium)]
        public void Warm(int count)
        {
            if (!enabled)
                return;

            count = Mathf.Max(0, count);

            for (int i = 0; i < count; i++)
            {
                if (!CanCreateNew())
                    break;

                GameObject go = CreateInstance();
                go.SetActive(false);
                _available.Push(go);
            }
        }

        public bool TryGet(out GameObject instance, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null
        )
        {
            instance = null;
            
            if (!enabled)
                return false;
            
            if (_available.Count > 0)
                instance = _available.Pop();
            else
            {
                if (!CanCreateNew())
                    return false;

                instance = CreateInstance();
            }
            
            _inUse.Add(instance);

            if (parent != null)
                instance.transform.SetParent(parent, worldPositionStays: false);
            
            instance.transform.SetPositionAndRotation(position, rotation);

            if (instance.TryGetComponent(out IPooledGO pooled))
                pooled.OnGetFromPool();
            
            if (_setActiveOnGet)
                instance.SetActive(true);

            return true;
        }
        
        public bool TryGet(out IPooledGO instance, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null
        )
        {
            instance = null;
            GameObject go;
            
            if (!enabled)
                return false;
            
            if (_available.Count > 0)
                go = _available.Pop();
            else
            {
                if (!CanCreateNew())
                    return false;

                go = CreateInstance();
            }
            
            _inUse.Add(go);

            if (parent != null)
                go.transform.SetParent(parent, worldPositionStays: false);
            
            go.transform.SetPositionAndRotation(position, rotation);

            if (go.TryGetComponent(out instance))
                instance.OnGetFromPool();
            else
                instance = go.AddComponent<PooledGameObject>();
            
            if (_setActiveOnGet)
                go.SetActive(true);

            return true;
        }

        public GameObject Get(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if (!TryGet(out GameObject instance, position, rotation, parent))
                throw new InvalidOperationException($"{name} no pooled object available, can't make new ones.");
            return instance;
        }

        public void Return(GameObject instance)
        {
            if (!enabled || instance == null)
                return;

            if (!_inUse.Remove(instance))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Tried to return '{instance.name}' to pool '{name}', but it wasn't marked in-use " +
                                 "(foreign or double return).", instance);
#endif
                return;
            }
            
            if (_parentToPoolOnReturn)
                instance.transform.SetParent(transform, worldPositionStays: false);
            
            if (instance.TryGetComponent(out IPooledGO pooled))
                pooled.OnReturnToPool();
            
            _available.Push(instance);
        }

        [Button(ButtonSizes.Medium)]
        public void ClearAndDestroyAll()
        {
            if (_available == null || _inUse == null)
                return;

            while (_available.Count > 0)
            {
                GameObject go = _available.Pop();
                if (go != null)
                    Destroy(go);
            }
            
            foreach (GameObject go in _inUse)
                if (go != null) Destroy(go);
            
            _inUse.Clear();
            _totalCreated = 0;
        }
        
        private bool CanCreateNew()
        {
            if (_isPoolInfinite)
                return true;

            return _totalCreated < _maxPoolSize;
        }

        private GameObject CreateInstance()
        {
            GameObject go = Instantiate(_prefab, transform);
            go.name = $"{_prefab.name} (Pooled)";
            
            IPooledGO pooled = go.GetComponent<IPooledGO>();
            if (pooled == null)
                pooled = go.AddComponent<PooledGameObject>();

            pooled.SetOwnerPool(this);
            
            _totalCreated++;
            return go;
        }
    }
}