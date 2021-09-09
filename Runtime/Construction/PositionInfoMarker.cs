using System;
using System.Collections.Generic;
using System.Linq;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class PositionInfoMarker
    {
        public int x;
        public int y;
        public bool reachable;
        public bool behindTransition;
        public Direction transition = Direction.None;
        public bool transitionAhead;
        public Direction aheadDirection = Direction.None;
        public int aheadDistance;
        public Direction backDirection = Direction.None;
        public int backDistance;
        public bool[] explicitNoSpawn; // indexed by BasicEntity.Direction enum
        public bool[] closedSides; // indexed by BasicEntity.Direction enum
        public bool containsTrigger;

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

        public PositionInfoMarker(int x, int y, bool reachable) : this()
        {
            this.x = x;
            this.y = y;
            this.reachable = reachable;
        }

        public PositionInfoMarker(PositionInfoMarker copyFrom) : this()
        {
            x = copyFrom.x;
            y = copyFrom.y;
            reachable = copyFrom.reachable;
            behindTransition = copyFrom.behindTransition;
            transition = copyFrom.transition;
            transitionAhead = copyFrom.transitionAhead;
            aheadDirection = copyFrom.aheadDirection;
            aheadDistance = copyFrom.aheadDistance;
            backDirection = copyFrom.backDirection;
            backDistance = copyFrom.backDistance;
            containsTrigger = copyFrom.containsTrigger;

            // TODO: incomplete
        }

        public void MarkAllSidesNoSpawn(bool state = true)
        {
            for (int i = 0; i < explicitNoSpawn.Length; i++)
            {
                explicitNoSpawn[i] = state;
            }
        }

        public bool HasNoSpawningSides()
        {
            return explicitNoSpawn.All(e => e);
        }

        public override string ToString()
        {
            return $"PIM ({x},{y}) (reachable: {reachable}, ahead distance: {aheadDistance})";
        }
    }
}