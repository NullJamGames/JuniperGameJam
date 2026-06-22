using System.Collections.Generic;
using NJG.Runtime.Events;
using NJG.Utilities;
using UnityEngine;

namespace NJG.Runtime.Map
{
    public class ChunkSpawner
    {
        private readonly ChunkSpawnOptions _spawnOptions;
        private readonly List<HallwayChunk> _activeChunks = new ();

        public ChunkSpawner(ChunkSpawnOptions spawnOptions)
        {
            _spawnOptions = spawnOptions;
        }

        public void SpawnInitialChunks()
        {
            Vector3 newChunkPosition = new (0f, 0f, -_spawnOptions.SpawnIntervalZ);
            for (int x = 0; x < _spawnOptions.ChunksToLoad; x++)
            {
                _activeChunks.Add(SpawnRandomChunk(newChunkPosition));
                newChunkPosition = newChunkPosition.WithZ(newChunkPosition.z + _spawnOptions.SpawnIntervalZ);
            }
        }
        
        public void SpawnNextChunk()
        {
            HallwayChunk lastChunk = _activeChunks[^1];
            Vector3 spawnPosition = lastChunk.transform.position + new Vector3(0f, 0f, _spawnOptions.SpawnIntervalZ);
            HallwayChunk newChunk = SpawnRandomChunk(spawnPosition);
            _activeChunks.Add(newChunk);
            
            if (_activeChunks.Count > _spawnOptions.ChunksToLoad)
            {
                HallwayChunk oldChunk = _activeChunks[0];
                _activeChunks.RemoveAt(0);
                
                // TODO: Instead of destroying old chunks, we could maybe reuse them if performance is an issue.
                Object.Destroy(oldChunk.gameObject);
            }
            
            if (newChunk.transform.position.z >= _spawnOptions.MaxChunkZPosition)
            {
                foreach (HallwayChunk chunk in _activeChunks)
                    chunk.transform.position = new Vector3(0f, 0f, chunk.transform.position.z - _spawnOptions.MaxChunkZPosition);
                
                EventBus.TriggerEvent(new ChunkZPositionResetEvent(_spawnOptions.MaxChunkZPosition));
            }
        }

        public float GetChunkTriggerPositionZ()
        {
            return _activeChunks[2].transform.position.z;
        }

        public Vector3 GetGroundPositionReference()
        {
            return _activeChunks[1].transform.position;
        }
        
        private HallwayChunk SpawnRandomChunk(Vector3 position)
        {
            // TODO: We can make this more performant by using a pool.
            HallwayChunk chunkPrefab = _spawnOptions.HallwayChunks[Random.Range(0, _spawnOptions.HallwayChunks.Length)];
            return Object.Instantiate(chunkPrefab, position, Quaternion.identity);
        }
    }
}