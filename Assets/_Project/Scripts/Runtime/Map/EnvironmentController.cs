using NJG.Runtime.Events;
using NJG.Runtime.Managers;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Map
{
    public class EnvironmentController : MonoBehaviour
    {
        [FoldoutGroup("Ground"), SerializeField]
        private Transform _groundCollider;

        [FoldoutGroup("Options"), SerializeField, InlineEditor]
        private ChunkSpawnOptions _chunkSpawnOptions;

        private ChunkSpawner _chunkSpawner;

        private void Awake()
        {
            _chunkSpawner = new ChunkSpawner(_chunkSpawnOptions);
        }

        private void OnEnable()
        {
            EventBus.StartListening<RequestNextChunkEvent>(OnNextChunkRequest);
        }

        private void Start()
        {
            _chunkSpawner.SpawnInitialChunks();
            GameManager.Instance.UpdateEnvironmentChunkTriggerZ(_chunkSpawner.GetChunkTriggerPositionZ());
        }
        
        private void OnDisable()
        {
            EventBus.StopListening<RequestNextChunkEvent>(OnNextChunkRequest);
        }

        private void OnNextChunkRequest(RequestNextChunkEvent e)
        {
            _chunkSpawner.SpawnNextChunk();
            _groundCollider.position = _chunkSpawner.GetGroundPositionReference();
            GameManager.Instance.UpdateEnvironmentChunkTriggerZ(_chunkSpawner.GetChunkTriggerPositionZ());
        }
    }
}