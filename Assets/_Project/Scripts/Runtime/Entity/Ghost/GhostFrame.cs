using System;
using System.Collections.Generic;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    [Serializable]
    public struct GhostFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;
        
        public GhostFrame(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    [Serializable]
    public class GhostRunData
    {
        public int Version = 1;
        public int Score;
        public int LevelSeed;
        public string PlayerName;
        public List<GhostFrame> Frames;
    }
}