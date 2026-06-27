using System.Collections.Generic;
using System.Linq;
using NJG.Runtime.Events;
using NJG.Utilities;
using UnityEngine;

namespace NJG.Runtime.Map
{
    public class ChunkSpawner
    {
        private readonly ChunkSpawnOptions _spawnOptions;
        private readonly List<HallwayChunk> _activeChunks = new ();
        private HallwayChunk _nextChunkPrefab;

        private readonly DeterministicRandom _random;

        public ChunkSpawner(ChunkSpawnOptions spawnOptions, int seed)
        {
            _spawnOptions = spawnOptions;
            _random = new DeterministicRandom(seed);
        }

        public void SpawnInitialChunks()
        {
            Vector3 newChunkPosition = new (0f, 0f, -_spawnOptions.SpawnIntervalZ);
            for (int x = 0; x < _spawnOptions.ChunksToLoad - 1; x++)
            {
                _activeChunks.Add(SpawnRandomChunk(newChunkPosition, true));
                newChunkPosition = newChunkPosition.WithZ(newChunkPosition.z + _spawnOptions.SpawnIntervalZ);
            }
            SpawnNextChunk();
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

        public void ResetChunks(int seed)
        {
            foreach (HallwayChunk chunk in _activeChunks)
                Object.Destroy(chunk.gameObject);
            
            _activeChunks.Clear();
            _nextChunkPrefab = null;

            _random.Reset(seed);
            
            SpawnInitialChunks();
        }

        public float GetChunkTriggerPositionZ()
        {
            return _activeChunks[2].transform.position.z;
        }

        public Vector3 GetGroundPositionReference()
        {
            return _activeChunks[1].transform.position;
        }
        
        private HallwayChunk SpawnRandomChunk(Vector3 position, bool isEmpty = false)
        {
            if (_nextChunkPrefab == null)
                _nextChunkPrefab = QueNextChunkPrefab(3, isEmpty);
            
            HallwayChunk chunkPrefab = _nextChunkPrefab;
            _nextChunkPrefab = QueNextChunkPrefab(_nextChunkPrefab.NumberOfLanes, isEmpty);
            
            // HallwayChunk chunkPrefab;
            // if (isEmpty)
            // {
            //     HallwayChunk[] chunks = _spawnOptions.HallwayChunks.Where(chunk => chunk.NumberOfLanes == 3).ToArray();
            //     chunkPrefab = chunks[_random.Range(0, chunks.Length)];
            // }
            // else
            // {
            //     int nextChunkLanes = GetNextChunkLanes(lastChunkLanes);
            //     HallwayChunk[] chunks = _spawnOptions.HallwayChunks.Where(chunk => chunk.NumberOfLanes == nextChunkLanes).ToArray();
            //     chunkPrefab = chunks[_random.Range(0, chunks.Length)];
            // }
            
            // TODO: We can make this more performant by using a pool.
            //HallwayChunk chunkPrefab = _spawnOptions.HallwayChunks[_random.Range(0, _spawnOptions.HallwayChunks.Length)];
            HallwayChunk newChunk = Object.Instantiate(chunkPrefab, position, Quaternion.identity);
            newChunk.Init(_nextChunkPrefab.NumberOfLanes, isEmpty, _random);
            return newChunk;
        }

        private HallwayChunk QueNextChunkPrefab(int lastChunkLanes, bool isEmpty)
        {
            if (isEmpty)
            {
                HallwayChunk[] chunks = _spawnOptions.HallwayChunks.Where(chunk => chunk.NumberOfLanes == 3).ToArray();
                return chunks[_random.Range(0, chunks.Length)];
            }
            else
            {
                int nextChunkLanes = GetNextChunkLanes(lastChunkLanes);
                HallwayChunk[] chunks = _spawnOptions.HallwayChunks.Where(chunk => chunk.NumberOfLanes == nextChunkLanes).ToArray();
                return chunks[_random.Range(0, chunks.Length)];
            }
        }

        private int GetNextChunkLanes(int currentChunkLanes)
        {
            if (currentChunkLanes != 3)
                return 3;
            
            float roll = _random.Range(0f, 1f);

            float chanceFor1or5 = 0.5f;
            if (roll < chanceFor1or5)
                return _random.Range(0f, 1f) < 0.5f ? 1 : 5;

            return 3;

            // return currentChunkLanes switch
            // {
            //     1 or 5 => 3,
            //
            //     2 => roll < 0.10f ? 1 :
            //         roll < 0.325f ? 2 : 3,
            //
            //     3 => roll < 0.75f ? 3 :
            //         roll < 0.875f ? 2 : 4,
            //
            //     4 => roll < 0.10f ? 5 :
            //         roll < 0.325f ? 4 : 3,
            //
            //     _ => 3
            // };
        }
    }
}