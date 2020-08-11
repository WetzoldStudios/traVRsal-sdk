using System;
using System.Collections.Generic;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class PositionInfoMarker
    {
        public bool reachable;
        public bool explicitNoSpawn;
        public Direction transition = Direction.None;
        public bool transitionAhead;
        public Direction aheadDirection = Direction.None;
        public int aheadDistance;
        public Direction backDirection = Direction.None;
        public int backDistance;
        public bool[] closedSides; // indexed by BasicEntity.Direction enum
        public Direction cornerDirection = Direction.None;

        // spawning info
        public bool[] spawnedSides;
        public List<string> spawnedObjects;
        public List<string> spawnedLayers;

        public PositionInfoMarker()
        {
            closedSides = new bool[6];
            spawnedSides = new bool[6];
            spawnedObjects = new List<string>();
            spawnedLayers = new List<string>();
        }

        public PositionInfoMarker(bool reachable, bool explicitNoSpawn) : this()
        {
            this.reachable = reachable;
            this.explicitNoSpawn = explicitNoSpawn;
        }

        public override string ToString()
        {
            return $"PIM (reachable: {reachable}, no-spawn: {explicitNoSpawn})";
        }
    }
}