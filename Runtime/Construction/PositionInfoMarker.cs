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
        public Direction transition = Direction.NONE;
        public bool transitionAhead;
        public Direction aheadDirection = Direction.NONE;
        public int aheadDistance;
        public Direction backDirection = Direction.NONE;
        public int backDistance;
        public bool[] closedSides; // indexed by BasicEntity.Direction enum
        public Direction cornerDirection = Direction.NONE;

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