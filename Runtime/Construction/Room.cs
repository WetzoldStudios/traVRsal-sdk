using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Room
    {
        [Header("Configuration")]
        public string name;
        public Vector2Int minSize = new Vector2Int(4, 4);
        public List<Floor> floors;
        public bool isExit = false;
        public bool isIntro = false;
        public bool invisibleGround = false;
        public bool blockAgents = true;
        public Color ambientColor = new Color(0.5f, 0.482f, 0.445f);
        public Color lightColor = Color.white;
        public float lightIntensity = 1f;
        public string music;
        public TMProperty[] properties;

        [Header("Runtime Information")]
        public Vector2Int curSize = BasicEntity.EMPTY;
        public string scenePath;
        public bool musicPlayed = false;
        public int agentCount = 0;
        public int idx;
        public byte stencilId = 0;
        public int layerIdx = 0;
        public int navAgentId;
        public Transform node;
        public Bounds bounds;
        public Transform center;

        // cache structures
        [HideInInspector]
        [NonSerialized]
        public Dictionary<string, Material> materialsCache;
        [HideInInspector]
        [NonSerialized]
        public List<Renderer> rendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Renderer> transparentRendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Renderer> untouchedRendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Renderer> sceneryRendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Renderer> movingRendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Light> lightCache;
        [HideInInspector]
        [NonSerialized]
        public Dictionary<int, List<Renderer>> portalVisibleRendererCache;
        [HideInInspector]
        [NonSerialized]
        public List<Behaviour> portalCache;
        [HideInInspector]
        [NonSerialized]
        public HashSet<int> reachableRooms;
        [HideInInspector]
        [NonSerialized]
        public HashSet<int> backSortedRooms;
        [HideInInspector]
        [NonSerialized]
        public List<Socket> sockets;
        [HideInInspector]
        [NonSerialized]
        public List<GameObject> socketItems;

        public Room()
        {
            floors = new List<Floor>();
        }

        public Room(string name) : this()
        {
            this.name = name;
        }

        public Room(Room copyFrom) : this()
        {
            name = copyFrom.name;
            minSize = copyFrom.minSize;
            curSize = copyFrom.curSize;
            scenePath = copyFrom.scenePath;
            invisibleGround = copyFrom.invisibleGround;
            ambientColor = copyFrom.ambientColor;
            lightColor = copyFrom.lightColor;
            lightIntensity = copyFrom.lightIntensity;
            isExit = copyFrom.isExit;
            isIntro = copyFrom.isIntro;
            music = copyFrom.music;

            foreach (Floor floor in copyFrom.floors)
            {
                floors.Add(new Floor(floor));
            }
        }

        public override string ToString()
        {
            return $"Room {name} (index: {idx}, layer: {layerIdx})";
        }
    }

}