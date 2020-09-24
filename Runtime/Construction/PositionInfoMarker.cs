using System;
using System.Collections.Generic;
using System.Linq;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class PositionInfoMarker
    {
        public bool reachable;
        public Direction transition = Direction.None;
        public bool transitionAhead;
        public Direction aheadDirection = Direction.None;
        public int aheadDistance;
        public Direction backDirection = Direction.None;
        public int backDistance;
        public bool[] explicitNoSpawn; // indexed by BasicEntity.Direction enum
        public bool[] closedSides; // indexed by BasicEntity.Direction enum

        // spawning info
        public bool[] spawnedSides;
        public List<string> spawnedObjects;
        public List<string> spawnedLayers;

        public PositionInfoMarker()
        {
            explicitNoSpawn = new bool[6];
            closedSides = new bool[6];
            spawnedSides = new bool[6];
            spawnedObjects = new List<string>();
            spawnedLayers = new List<string>();
        }

        public PositionInfoMarker(bool reachable) : this()
        {
            this.reachable = reachable;
        }

        public void MarkAllSidesNoSpawn()
        {
            for (int i = 0; i < explicitNoSpawn.Length; i++)
            {
                explicitNoSpawn[i] = true;
            }
        }

        public bool HasNoSpawningSides()
        {
            return explicitNoSpawn.All(e => e);
        }

        public override string ToString()
        {
            return $"PIM (reachable: {reachable}, ahead distance: {aheadDistance})";
        }
    }
}