using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Zone
    {
        public enum TimeResetMode
        {
            OnZoneEntryAndDeath = 0,
            OnZoneEntry = 1
        }

        [Header("Configuration")] public string name;
        public Vector2Int minSize = new Vector2Int(4, 4);
        public int availableTime;
        public TimeResetMode timeResetMode = TimeResetMode.OnZoneEntryAndDeath;
        public string chapter;
        public bool createCheckpoint;
        public bool isExit;
        public bool isIntro;
        public bool blockAgents = true;
        public bool blockNavigation = true;
        public bool reactivateTransitions;
        public bool invisibleGround;
        public string variable;

        [Header("Environment")] public Color ambientColor = new Color(0.8f, 0.782f, 0.745f);
        public Color lightColor = Color.white;
        public Color backgroundColor = Color.black;
        public float lightIntensity = 1f;
        public string skybox;
        public float fogDensity = 0.02f;

        [Header("Audio")] public string music;
        public float musicVolume = 1f;
        public string randomAmbience;
        public string randomMusic;

        [Header("Construction")] public Vector3 offset = Vector3.zero;
        public Vector3 offsetRaw = Vector3.zero;
        public List<Floor> floors;
        public TMProperty[] properties;

        [Header("Runtime Information")] public Vector2Int curSize = BasicEntity.EMPTY;
        public string scenePath;
        public bool musicPlayed;
        public bool hasLightColor;
        public bool hasAmbientColor;
        public bool hasBackgroundColor;
        public bool hasLightIntensity;
        public bool hasFog;
        public int agentCount;
        public int idx;
        public byte stencilId;
        public int layerIdx;
        public int navAgentId;
        public string originalName;
        public string variationOf;
        public float entryTime;
        public float remainingTime;

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
        [NonSerialized] public Dictionary<int, List<Renderer>> foreignVisibleRendererCache;
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

        public void ClearRenderCaches()
        {
            // remove all renderer caches but leave logic caches in-tact to not break downstream logic
            materialsCache = null;
            rendererCache = null;
            transparentRendererCache = null;
            untouchedRendererCache = null;
            sceneryRendererCache = null;
            movingRendererCache = null;
            lightCache = null;
            foreignVisibleRendererCache = null;
        }

        public Zone(Zone copyFrom) : this()
        {
            name = copyFrom.name;
            minSize = copyFrom.minSize;
            curSize = copyFrom.curSize;
            offset = copyFrom.offset;
            offsetRaw = copyFrom.offsetRaw;
            scenePath = copyFrom.scenePath;
            availableTime = copyFrom.availableTime;
            hasAmbientColor = copyFrom.hasAmbientColor;
            hasBackgroundColor = copyFrom.hasBackgroundColor;
            hasLightColor = copyFrom.hasLightColor;
            hasLightIntensity = copyFrom.hasLightIntensity;
            hasFog = copyFrom.hasFog;
            ambientColor = copyFrom.ambientColor;
            lightColor = copyFrom.lightColor;
            lightIntensity = copyFrom.lightIntensity;
            fogDensity = copyFrom.fogDensity;
            backgroundColor = copyFrom.backgroundColor;
            chapter = copyFrom.chapter;
            createCheckpoint = copyFrom.createCheckpoint;
            isExit = copyFrom.isExit;
            isIntro = copyFrom.isIntro;
            reactivateTransitions = copyFrom.reactivateTransitions;
            invisibleGround = copyFrom.invisibleGround;
            music = copyFrom.music;
            randomMusic = copyFrom.randomMusic;
            skybox = copyFrom.skybox;
            blockAgents = copyFrom.blockAgents;
            blockNavigation = copyFrom.blockNavigation;
            variationOf = copyFrom.variationOf;
            variable = copyFrom.variable;
            originalName = copyFrom.originalName;
            properties = SDKUtil.CopyProperties(copyFrom.properties);

            foreach (Floor floor in copyFrom.floors)
            {
                Floor copy = new Floor(floor);
                copy.positionInfoMarker = floor.positionInfoMarker; // FIXME: quick-fix for retaining challenge mode correctly until copy-constructor for position info exists

                floors.Add(copy);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Zone zone &&
                   name == zone.name &&
                   chapter == zone.chapter &&
                   createCheckpoint == zone.createCheckpoint &&
                   minSize.Equals(zone.minSize) &&
                   offset.Equals(zone.offset) &&
                   // FIXME: returns false for some reason
                   EqualityComparer<List<Floor>>.Default.Equals(floors, zone.floors) &&
                   isExit == zone.isExit &&
                   isIntro == zone.isIntro &&
                   reactivateTransitions == zone.reactivateTransitions &&
                   invisibleGround == zone.invisibleGround &&
                   blockAgents == zone.blockAgents &&
                   blockNavigation == zone.blockNavigation &&
                   availableTime == zone.availableTime &&
                   hasAmbientColor == zone.hasAmbientColor &&
                   hasBackgroundColor == zone.hasBackgroundColor &&
                   hasLightColor == zone.hasLightColor &&
                   hasLightIntensity == zone.hasLightIntensity &&
                   hasFog == zone.hasFog &&
                   ambientColor.Equals(zone.ambientColor) &&
                   lightColor.Equals(zone.lightColor) &&
                   backgroundColor.Equals(zone.backgroundColor) &&
                   lightIntensity == zone.lightIntensity &&
                   fogDensity == zone.fogDensity &&
                   skybox == zone.skybox &&
                   music == zone.music &&
                   randomMusic == zone.randomMusic &&
                   variable == zone.variable &&
                   EqualityComparer<TMProperty[]>.Default.Equals(properties, zone.properties);
        }

        public override int GetHashCode()
        {
            int hashCode = -1742729958;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(chapter);
            hashCode = hashCode * -1521134295 + minSize.GetHashCode();
            hashCode = hashCode * -1521134295 + offset.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Floor>>.Default.GetHashCode(floors);
            hashCode = hashCode * -1521134295 + isExit.GetHashCode();
            hashCode = hashCode * -1521134295 + isIntro.GetHashCode();
            hashCode = hashCode * -1521134295 + createCheckpoint.GetHashCode();
            hashCode = hashCode * -1521134295 + reactivateTransitions.GetHashCode();
            hashCode = hashCode * -1521134295 + invisibleGround.GetHashCode();
            hashCode = hashCode * -1521134295 + availableTime.GetHashCode();
            hashCode = hashCode * -1521134295 + blockAgents.GetHashCode();
            hashCode = hashCode * -1521134295 + blockNavigation.GetHashCode();
            hashCode = hashCode * -1521134295 + hasAmbientColor.GetHashCode();
            hashCode = hashCode * -1521134295 + hasLightColor.GetHashCode();
            hashCode = hashCode * -1521134295 + hasBackgroundColor.GetHashCode();
            hashCode = hashCode * -1521134295 + hasLightIntensity.GetHashCode();
            hashCode = hashCode * -1521134295 + hasFog.GetHashCode();
            hashCode = hashCode * -1521134295 + ambientColor.GetHashCode();
            hashCode = hashCode * -1521134295 + lightColor.GetHashCode();
            hashCode = hashCode * -1521134295 + backgroundColor.GetHashCode();
            hashCode = hashCode * -1521134295 + lightIntensity.GetHashCode();
            hashCode = hashCode * -1521134295 + fogDensity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(skybox);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(music);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(randomMusic);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(variable);
            hashCode = hashCode * -1521134295 + EqualityComparer<TMProperty[]>.Default.GetHashCode(properties);
            return hashCode;
        }

        public override string ToString()
        {
            return $"Zone '{name}'";
        }
    }
}