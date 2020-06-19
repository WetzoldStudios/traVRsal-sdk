using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Floor
    {
        [Header("Configuration")]
        public string name;
        public float height = 2.5f;
        public bool underground = false;
        public List<BasicEntity> entities;
        public Dictionary<string, TMProperty[]> layerProps;
        public TMProperty[] properties;

        [Header("Runtime Information")]
        public int idx;
        public PositionInfoMarker[,] positionInfoMarker;
        public Transform node;
        public Bounds bounds;
        public Transform center;

        [HideInInspector]
        [NonSerialized]
        public Maze maze;
        [HideInInspector]
        [NonSerialized]
        public List<Behaviour> portalCache;

        public Floor()
        {
            entities = new List<BasicEntity>();
            layerProps = new Dictionary<string, TMProperty[]>();
        }

        public Floor(string name) : this()
        {
            this.name = name;
        }

        public Floor(Floor copyFrom) : this()
        {
            name = copyFrom.name;
            height = copyFrom.height;
            underground = copyFrom.underground;

            foreach (BasicEntity entity in copyFrom.entities)
            {
                entities.Add(new BasicEntity(entity));
            }
        }

        public override string ToString()
        {
            return $"Floor {idx} ({height}m)";
        }
    }
}