using System.Linq;
using UnityEngine;

namespace NJG.Runtime.Map
{
    public class HallwaySegment
    {
        public struct LanePosition
        {
            public readonly float X;
            public readonly float Z;
            
            public LanePosition(float x, float z)
            {
                X = x;
                Z = z;
            }
        }
        
        private class Lane
        {
            public LanePosition Position;
            public bool IsEmpty;
        }
        
        private readonly float _segmentLength;
        private readonly Lane[] _lanes;
        
        public bool HasRamp { get; private set; }
        
        public HallwaySegment(int lanes, float laneIncrement, float zPosition, float segmentLength)
        {
            _lanes = new Lane[lanes];
            _segmentLength = segmentLength;
            float laneStartX = -(Mathf.FloorToInt(lanes / 2f)) * laneIncrement;
            for (int i = 0; i < lanes; i++)
            {
                _lanes[i] = new Lane
                {
                    Position = new LanePosition(laneStartX + i * laneIncrement, zPosition), IsEmpty = true
                };
            }
        }
        
        public bool TryGetRandomValidObstaclePosition(bool lastSegmentHasRamp, out LanePosition position)
        {
            position = default(LanePosition);
            if (!CanPlaceObstacle(lastSegmentHasRamp ? 1 : 2))
                return false;
            
            Lane[] emptyLanes = _lanes.Where(lane => lane.IsEmpty).ToArray();
            Lane emptyLane = emptyLanes[Random.Range(0, emptyLanes.Length)];
            emptyLane.IsEmpty = false;
            position = emptyLane.Position;
            return true;
        }

        public bool TryGetValidRampPosition(out LanePosition position)
        {
            position = default(LanePosition);
            Lane emptyLane = _lanes.FirstOrDefault(lane => lane.IsEmpty);
            if (emptyLane == null)
                return false;
            
            emptyLane.IsEmpty = false;
            HasRamp = true;
            position = emptyLane.Position;
            return true;
        }

        public LanePosition GetValidRandomPowerUpPosition(float segmentOffset)
        {
            Lane lane = _lanes[Random.Range(0, _lanes.Length)];
            float zPos = Random.Range(lane.Position.Z - _segmentLength + segmentOffset, lane.Position.Z - segmentOffset);
            return new LanePosition(lane.Position.X, zPos);
        }
        
        private bool CanPlaceObstacle(int minEmptySpots)
        {
            return _lanes.Count(lane => lane.IsEmpty) >= minEmptySpots;
        }
    }
}