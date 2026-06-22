using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NJG.Runtime.Map
{
    public class HallwayChunk : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _obstaclePrefabs;
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _powerUpPrefabs;

        [FoldoutGroup("Settings"), SerializeField]
        private int _minObstacles = 0;
        [FoldoutGroup("Settings"), SerializeField]
        private int _maxObstacles = 4;
        [FoldoutGroup("Settings"), SerializeField]
        private float _hallwayLength = 100f;

        [FoldoutGroup("Cluster Limits"), SerializeField]
        [Tooltip("Maximum number of obstacles allowed within the cluster interval of each other")]
        private int _maxObstaclesPerInterval = 2;
        [FoldoutGroup("Cluster Limits"), SerializeField]
        [Tooltip("Z distance range that defines 'next to each other' — obstacles within this distance count toward the cluster limit")]
        private float _clusterInterval = 10f;
        [FoldoutGroup("Cluster Limits"), SerializeField]
        [Tooltip("Minimum X distance between obstacles that are within the cluster interval — prevents overlapping")]
        private float _minObstacleXDistance = 5f;
        [FoldoutGroup("Cluster Limits"), SerializeField]
        [Tooltip("How many times to retry finding a valid spawn position before giving up on that obstacle")]
        private int _maxSpawnAttempts = 20;
        
        private void Start()
        {
            SpawnObstacles();
        }

        private void SpawnObstacles()
        {
            int obstaclesToSpawn = Random.Range(_minObstacles, _maxObstacles + 1);
            List<Vector2> spawnedPositions = new (); // x = world X, y = local Z

            for (int x = 0; x < obstaclesToSpawn; x++)
            {
                bool placed = false;

                for (int attempt = 0; attempt < _maxSpawnAttempts; attempt++)
                {
                    float candidateZ = Random.Range(5f, _hallwayLength - 5f);
                    float candidateX = Random.Range(-5f, 5f);

                    // Validate against all already-placed obstacles
                    int nearbyCount = 0;
                    bool tooCloseX = false;
                    foreach (Vector2 pos in from pos in spawnedPositions let withinCluster = 
                                 Mathf.Abs(pos.y - candidateZ) <= _clusterInterval where withinCluster select pos)
                    {
                        nearbyCount++;
                        if (Mathf.Abs(pos.x - candidateX) < _minObstacleXDistance)
                        {
                            tooCloseX = true;
                            break;
                        }
                    }

                    if (nearbyCount < _maxObstaclesPerInterval && !tooCloseX)
                    {
                        Vector3 spawnPosition = transform.position + new Vector3(candidateX, 0f, candidateZ);
                        GameObject obstaclePrefab = _obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)];
                        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, transform);
                        spawnedPositions.Add(new Vector2(candidateX, candidateZ));
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                    Debug.LogWarning($"[HallwayChunk] Could not find a valid spawn position for obstacle {x} after {_maxSpawnAttempts} attempts.");
            }
        }
    }
}