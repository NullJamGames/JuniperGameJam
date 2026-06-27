using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Map
{
    public class HallwayChunk : MonoBehaviour
    {
        public enum HallwayLanePosition
        {
            Left,
            Middle,
            Right
        }
        
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _obstaclePrefabs;
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _rampPrefabs;
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _powerUpPrefabs;
        [FoldoutGroup("Dependencies"), SerializeField]
        private GameObject[] _endLaneBlockers;

        [FoldoutGroup("General"), SerializeField]
        private float _hallwayLength = 150f;
        [FoldoutGroup("General"), SerializeField]
        private int _numberOfLanes = 3;
        [FoldoutGroup("General"), SerializeField]
        private HallwayLanePosition _lanePosition = HallwayLanePosition.Middle;
        [FoldoutGroup("General"), SerializeField]
        private float _laneIncrement = 5f;
        
        [FoldoutGroup("Segment"), SerializeField]
        private float _segmentIntervals = 50f;
        [FoldoutGroup("Segment"), SerializeField]
        private int _minObstacles = 1;
        [FoldoutGroup("Segment"), SerializeField, Range(0f, 1f)]
        private float _rampChance = 0.2f;
        [FoldoutGroup("Segment"), SerializeField, Range(0f, 1f)]
        private float _powerUpChance = 0.25f;
        [FoldoutGroup("Segment"), SerializeField]
        private float _powerUpSegmentOffset = 20f;

        private HallwaySegment[] _hallwaySegments;
        
        private DeterministicRandom _random;
        
        public int NumberOfLanes => _numberOfLanes;
        public int NextChunkLanes { get; private set; }

        public void Init(int nextChunkLanes, bool isEmpty, DeterministicRandom random)
        {
            NextChunkLanes = nextChunkLanes;
            _random = random;
            SetupEndBlockers();
            
            if (isEmpty)
                return;
            
            SpawnSegmentObjects();
            SpawnPowerUps();
        }

        private void SetupEndBlockers()
        {
            if (NextChunkLanes < NumberOfLanes)
            {
                foreach (GameObject endLaneBlocker in _endLaneBlockers)
                    endLaneBlocker.SetActive(true);
            }
        }

        // Here we spawn Obstacles and Ramps
        private void SpawnSegmentObjects()
        {
            int spawnIterations = Mathf.FloorToInt(_hallwayLength / _segmentIntervals);
            _hallwaySegments = new HallwaySegment[spawnIterations];
            HallwaySegment lastSegment = null;
            for (int x = 0; x < spawnIterations; x++)
            {
                _hallwaySegments[x] = new HallwaySegment(_numberOfLanes, _laneIncrement, 
                    ((x + 1) * _segmentIntervals) + transform.position.z, _segmentIntervals);
                bool lastSegmentHasRamp = lastSegment is { HasRamp: true };

                // We don't want to spawn obstacles and ramps on the last segment if the next chunk has less lanes.
                if (x == spawnIterations - 1 && NextChunkLanes < NumberOfLanes)
                    return;
                
                SpawnSegmentObstacles(_hallwaySegments[x], lastSegmentHasRamp, _random.Range(_minObstacles, _numberOfLanes + 1));
                
                // ramps
                if (_random.Range(0f, 1f) < _rampChance)
                {
                    SpawnSegmentRamp(_hallwaySegments[x]);
                }
                
                lastSegment = _hallwaySegments[x];
            }
        }

        private void SpawnSegmentObstacles(HallwaySegment segment, bool lastSegmentHasRamp, int amount)
        {
            for (int x = 0; x < amount; x++)
            {
                if (!segment.TryGetRandomValidObstaclePosition(lastSegmentHasRamp, _random, out HallwaySegment.LanePosition position))
                    break;
                
                GameObject obstacle = Instantiate(_obstaclePrefabs[_random.Range(0, _obstaclePrefabs.Length)], transform);
                Vector3 pos = new (position.X, 0f, position.Z);
                Quaternion rot = Quaternion.Euler(0f, 180f, 0f);
                obstacle.transform.SetPositionAndRotation(pos, rot);
            }
        }

        private void SpawnSegmentRamp(HallwaySegment segment)
        {
            if (!segment.TryGetValidRampPosition(out HallwaySegment.LanePosition rampPosition))
                return;
            
            GameObject ramp = Instantiate(_rampPrefabs[_random.Range(0, _rampPrefabs.Length)], transform);
            ramp.transform.position = new Vector3(rampPosition.X, 0f, rampPosition.Z);
        }

        private void SpawnPowerUps()
        {
            foreach (HallwaySegment segment in _hallwaySegments)
            {
                if (_random.Range(0f, 1f) > _powerUpChance)
                    continue;
                
                HallwaySegment.LanePosition position = segment.GetValidRandomPowerUpPosition(_powerUpSegmentOffset, _random);
                GameObject powerUp = Instantiate(_powerUpPrefabs[_random.Range(0, _powerUpPrefabs.Length)], transform);
                powerUp.transform.position = new Vector3(position.X, 0f, position.Z);
            }
        }
    }
}