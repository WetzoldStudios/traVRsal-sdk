using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Zone
    {
        [Header("Configuration")] public string name;
        public Vector2Int minSize = new Vector2Int(4, 4);
        public List<Floor> floors;
        public bool isExit;
        public bool isIntro;
        public bool invisibleGround;
        public bool blockAgents = true;
        public Color ambientColor = new Color(0.8f, 0.782f, 0.745f);
        public Color lightColor = Color.white;
        public Color backgroundColor = Color.black;
        public float lightIntensity = 1f;
        public string skybox;
        public string music;
        public string randomAmbience;
        public string randomMusic;
        public TMProperty[] properties;

        [Header("Runtime Information")] public Vector2Int curSize = BasicEntity.EMPTY;
        public string scenePath;
        public bool musicPlayed;
        public int agentCount;
        public int idx;
        public byte stencilId;
        public int layerIdx;
        public int navAgentId;
        public string originalName;
        public string variationOf;

        // cache structures
        [NonSerialized] public Transform node;
        [NonSerialized] public Bounds bounds;
        [NonSerialized] public Transform center;
        [NonSerialized] public Dictionary<string, Material> materialsCache;
        [NonSerialized] public List<Renderer> rendererCache;
        [NonSerialized] public List<Renderer> transparentRendererCache;
        [NonSerialized] public List<Renderer> untouchedRendererCache;
        [NonSerialized] public List<Renderer> sceneryRendererCache;
        [NonSerialized] public List<Renderer> movingRendererCache;
        [NonSerialized] public List<Light> lightCache;
        [NonSerialized] public Dictionary<int, List<Renderer>> portalVisibleRendererCache;
        [NonSerialized] public List<Behaviour> portalCache;
        [NonSerialized] public HashSet<int> reachableZones;
        [NonSerialized] public HashSet<int> backSortedZones;
        [NonSerialized] public List<Socket> sockets;
        [NonSerialized] public List<GameObject> socketItems;
        [NonSerialized] public GameObject navBlockerNode;

        public Zone()
        {
            floors = new List<Floor>();
            portalCache = new List<Behaviour>();
            reachableZones = new HashSet<int>();
        }

        public Zone(string name) : this()
        {
            this.name = name;
            originalName = name;
        }

        public Zone(Zone copyFrom) : this()
        {
            name = copyFrom.name;
            minSize = copyFrom.minSize;
            curSize = copyFrom.curSize;
            scenePath = copyFrom.scenePath;
            ambientColor = copyFrom.ambientColor;
            lightColor = copyFrom.lightColor;
            lightIntensity = copyFrom.lightIntensity;
            backgroundColor = copyFrom.backgroundColor;
            isExit = copyFrom.isExit;
            isIntro = copyFrom.isIntro;
            invisibleGround = copyFrom.invisibleGround;
            music = copyFrom.music;
            randomMusic = copyFrom.randomMusic;
            skybox = copyFrom.skybox;
            blockAgents = copyFrom.blockAgents;
            variationOf = copyFrom.variationOf;
            originalName = copyFrom.originalName;
            properties = SDKUtil.CopyProperties(copyFrom.properties);

            foreach (Floor floor in copyFrom.floors)
            {
                floors.Add(new Floor(floor));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Zone zone &&
                   name == zone.name &&
                   minSize.Equals(zone.minSize) &&
                   // FIXME: returns false for some reason
                   EqualityComparer<List<Floor>>.Default.Equals(floors, zone.floors) &&
                   isExit == zone.isExit &&
                   isIntro == zone.isIntro &&
                   invisibleGround == zone.invisibleGround &&
                   blockAgents == zone.blockAgents &&
                   ambientColor.Equals(zone.ambientColor) &&
                   lightColor.Equals(zone.lightColor) &&
                   backgroundColor.Equals(zone.backgroundColor) &&
                   lightIntensity == zone.lightIntensity &&
                   skybox == zone.skybox &&
                   music == zone.music &&
                   randomMusic == zone.randomMusic &&
                   EqualityComparer<TMProperty[]>.Default.Equals(properties, zone.properties);
        }

        public override int GetHashCode()
        {
            int hashCode = -1742729958;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + minSize.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Floor>>.Default.GetHashCode(floors);
            hashCode = hashCode * -1521134295 + isExit.GetHashCode();
            hashCode = hashCode * -1521134295 + isIntro.GetHashCode();
            hashCode = hashCode * -1521134295 + invisibleGround.GetHashCode();
            hashCode = hashCode * -1521134295 + blockAgents.GetHashCode();
            hashCode = hashCode * -1521134295 + ambientColor.GetHashCode();
            hashCode = hashCode * -1521134295 + lightColor.GetHashCode();
            hashCode = hashCode * -1521134295 + backgroundColor.GetHashCode();
            hashCode = hashCode * -1521134295 + lightIntensity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(skybox);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(music);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(randomMusic);
            hashCode = hashCode * -1521134295 + EqualityComparer<TMProperty[]>.Default.GetHashCode(properties);
            return hashCode;
        }

        public override string ToString()
        {
            return $"Zone {name}";
        }
    }
}