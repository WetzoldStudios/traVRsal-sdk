﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static traVRsal.SDK.DataBinding;

namespace traVRsal.SDK
{
    [Serializable]
    public class World
    {
        public enum TargetMeasures
        {
            Time = 0,
            Distance = 4,
            Accuracy = 1,
            Targets = 2,
            Deaths = 3,
            Points = 5
        }

        public enum SkyboxMode
        {
            None = 0,
            Single = 1,
            Multiple = 2
        }

        public enum TargetVisiblity
        {
            Always = 1,
            EnteringZone = 0
        }

        [Header("Information")] public string key;
        public string version;
        public string name;
        [DefaultValue("Experience")] public string category = "Experience";
        public string shortDescription;
        public string longDescription;
        public string coverImage;
        public bool chaptersUnlocked;
        public List<Chapter> chapters;

        [Header("Configuration")] public bool isVirtual;
        public string minVersion;
        public string minSize;
        public string maxSize;
        [DefaultValue(300)] public int availableTime = 5 * 60;
        [DefaultValue(true)] public bool enableChallenges = true;
        public bool autoCheckPoints;
        public TargetMeasures[] measures = {TargetMeasures.Time, TargetMeasures.Distance};
        public TargetVisiblity targetVisibility = TargetVisiblity.EnteringZone;

        [Header("Items & HUD")] [DefaultValue(true)]
        public bool showHandHud = true;

        public HUDConfig[] customHandHuds;
        public Reference defaultHandHud;
        public Reference defaultHandHudMain;
        public Reference defaultHandHudSecondary;

        public List<string> inventoryItems;
        public string initialItemMain;
        public string initialItemSecondary;
        public string handBackItemMain;
        public string handBackItemSecondary;
        public string[] headItems;

        [Header("Assets")] public string deathSound;
        public string[] defaultRandomSkybox;
        public string defaultSkybox;
        public string defaultScenery;
        [DefaultValue("/Base/LightHall")] public string introScenery = "/Base/LightHall";

        [DefaultValue("/Base/LightHall-Outro")]
        public string outroScenery = "/Base/LightHall-Outro";

        [DefaultValue("/Base/FutureWorld_Resolution_Loop.ogg")]
        public string outroMusic = "/Base/FutureWorld_Resolution_Loop.ogg";

        [Header("Content")] public List<WorldDataReference> worldData;
        public bool disableJourneys;
        public List<Journey> journeys;
        public List<WorldSetting> settings;
        public List<ImageProvider> imageProviders;
        public List<Variable> initialVariables;
        public List<TextFragment> speech;
        public List<ReplacementRule> replacements;
        public List<SpawnRule> spawnRules;
        public MultiBehaviors behaviors;
        public List<Credit> credits;
        public List<string> customShaders;

        [Header("Runtime Data")] public bool journeyMode;
        public bool rotateWorld;
        public bool invalidSettings;
        public SkyboxMode skyboxMode = SkyboxMode.None;
        public List<ObjectSpec> objectSpecs;
        public List<string> dependencies;
        public List<string> worldDependencies;
        public List<Zone> zones;
        public List<Zone> zoneTemplates;
        public List<Journey> journeyTemplates;
        public UserWorld remoteMetaData;

        // cache structures
        [NonSerialized] public Texture2D cover;
        [NonSerialized] public Dictionary<string, int> visitedZones;
        [NonSerialized] public Dictionary<string, string> zoneTemplateCache;
        [NonSerialized] public Dictionary<string, Tuple<string, BasicEntity>> locationCache;
        [NonSerialized] public Dictionary<int, HashSet<int>> zoneVisibility;
        [NonSerialized] public Challenge challenge;
        [NonSerialized] public bool useExistingWorld;
        [NonSerialized] public int autoIdx = 1;
        [NonSerialized] public float tileSize;
        [NonSerialized] public Vector2Int minSizeV = Vector2Int.zero;
        [NonSerialized] public Vector2Int maxSizeV = Vector2Int.zero;

        public World()
        {
            chapters = new List<Chapter>();
            zones = new List<Zone>();
            zoneTemplates = new List<Zone>();
            spawnRules = new List<SpawnRule>();
            imageProviders = new List<ImageProvider>();
            replacements = new List<ReplacementRule>();
            objectSpecs = new List<ObjectSpec>();
            visitedZones = new Dictionary<string, int>();
            zoneTemplateCache = new Dictionary<string, string>();
            locationCache = new Dictionary<string, Tuple<string, BasicEntity>>();
            zoneVisibility = new Dictionary<int, HashSet<int>>();
            inventoryItems = new List<string>();
            speech = new List<TextFragment>();
            initialVariables = new List<Variable>();
            settings = new List<WorldSetting>();
            worldData = new List<WorldDataReference>();
            journeys = new List<Journey>();
            journeyTemplates = new List<Journey>();
            dependencies = new List<string>();
            worldDependencies = new List<string>();
            customShaders = new List<string>();
        }

        public World(string key) : this()
        {
            this.key = key;
        }

        public void NullifyEmpties()
        {
            if (chapters != null && chapters.Count == 0) chapters = null;
            if (headItems != null && headItems.Length == 0) headItems = null;
            if (defaultRandomSkybox != null && defaultRandomSkybox.Length == 0) defaultRandomSkybox = null;
            if (inventoryItems != null && inventoryItems.Count == 0) inventoryItems = null;
            if (zones != null && zones.Count == 0) zones = null;
            if (zoneTemplates != null && zoneTemplates.Count == 0) zoneTemplates = null;
            if (spawnRules != null && spawnRules.Count == 0) spawnRules = null;
            if (imageProviders != null && imageProviders.Count == 0) imageProviders = null;
            if (replacements != null && replacements.Count == 0) replacements = null;
            if (objectSpecs != null && objectSpecs.Count == 0) objectSpecs = null;
            if (visitedZones != null && visitedZones.Count == 0) visitedZones = null;
            if (zoneTemplateCache != null && zoneTemplateCache.Count == 0) zoneTemplateCache = null;
            if (locationCache != null && locationCache.Count == 0) locationCache = null;
            if (zoneVisibility != null && zoneVisibility.Count == 0) zoneVisibility = null;
            if (speech != null && speech.Count == 0) speech = null;
            if (initialVariables != null && initialVariables.Count == 0) initialVariables = null;
            if (settings != null && settings.Count == 0) settings = null;
            if (worldData != null && worldData.Count == 0) worldData = null;
            if (dependencies != null && dependencies.Count == 0) dependencies = null;
            if (worldDependencies != null && worldDependencies.Count == 0) worldDependencies = null;
            if (customShaders != null && customShaders.Count == 0) customShaders = null;
            if (credits != null && credits.Count == 0) credits = null;
            if (journeys != null && journeys.Count == 0) journeys = null;
            if (journeyTemplates != null && journeyTemplates.Count == 0) journeyTemplates = null;

            if (initialVariables != null)
            {
                foreach (Variable variable in initialVariables)
                {
                    if (variable.currentOrder != null && variable.currentOrder.Count == 0) variable.currentOrder = null;
                    if (variable.targetOrder != null && variable.targetOrder.Count == 0) variable.targetOrder = null;
                    if (variable.listeners != null && variable.listeners.Count == 0) variable.listeners = null;
                }
            }
        }

        public override string ToString()
        {
            return $"World {name}";
        }
    }
}