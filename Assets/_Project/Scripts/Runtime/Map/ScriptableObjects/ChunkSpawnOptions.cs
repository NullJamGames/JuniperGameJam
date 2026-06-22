using UnityEngine;

namespace NJG.Runtime.Map
{
    [CreateAssetMenu(fileName = "ChunkSpawnOptions", menuName = "NJG/Options/ChunkSpawnOptions")]
    public class ChunkSpawnOptions : ScriptableObject
    {
        [field: SerializeField]
        public HallwayChunk[] HallwayChunks { get; private set; }
        [field: SerializeField]
        public float SpawnIntervalZ { get; private set; } = 50f;
        [field: SerializeField, Tooltip("Number of chunks to load at at once (NOTE: Need to have at least 3)")]
        public int ChunksToLoad { get; private set; } = 5;
        [field: SerializeField]
        public float MaxChunkZPosition { get; private set; } = 10000f;
    }
}