using System;
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

        public enum TargetVisibility
        {
            Always = 1,
            EnteringZone = 0
        }

        public enum UnlockMethod
        {
            None = 0,
            ButtonCombination = 1
        }

        [Header("Presentation")] public string key;
        public string version;
        public int versionCode;
        public string name;
        [DefaultValue("Experience")] public string category = "Experience";
        public string shortDescription;
        public string longDescription;
        public string coverImage;
        public string trailer;

        [Header("Unlocking")] public UnlockMethod unlockMethod = UnlockMethod.None;
        public string unlockCombination;
        [DefaultValue("?????")] public string lockedName = "?????";
        public string lockedShortDescription;
        public string lockedLongDescription;
        public string unlockLink;
        public string unlockLinkTitle;

        [Header("Chapters")] public bool loadAllChapters;
        public string defaultChapter;
        public List<Chapter> chapters;

        [Header("Configuration")] public bool isVirtual;
        public string minAppVersion;
        [DefaultValue(1)] public int minCompatibilityVersionCode = 1;
        public string minSize;
        public string maxSize;
        public float minTileSize;
        public float maxTileSize;
        [DefaultValue(1.8f)] public float playerHeight = 1.8f;
        public int lives;
        public bool defaultConsiderPlayerHeight;
        [DefaultValue(3.5f)] public float maxFallHeight = 3.5f;
        [DefaultValue(300)] public int availableTime = 5 * 60;
        [DefaultValue(true)] public bool enableChallenges = true;
        public bool autoCheckPoints;
        [DefaultValue(true)] public bool createIntro = true;
        public TargetMeasures[] measures = {TargetMeasures.Time, TargetMeasures.Distance};
        public TargetVisibility targetVisibility = TargetVisibility.EnteringZone;
        public List<QualityRule> qualityRules;

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
        public bool firstSkyboxInIntro;
        public bool lastSkyboxInOutro;
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
        public bool requiresNavMesh;
        public bool invalidSettings;
        public string chapter;
        public SkyboxMode skyboxMode = SkyboxMode.None;
        public List<ObjectSpec> objectSpecs;
        public HashSet<string> usedTags;
        public List<Zone> zones;
        public List<Zone> zoneTemplates;
        public List<Journey> journeyTemplates;
        public UserWorld remoteMetaData;
        public WorldAnalysis dependencies;

        // cache structures
        [NonSerialized] public Texture2D cover;
        [NonSerialized] public bool loadingCover;
        [NonSerialized] public Dictionary<string, int> visitedZones;
        [NonSerialized] public Dictionary<string, string> zoneTemplateCache;
        [NonSerialized] public Dictionary<string, Tuple<string, BasicEntity>> locationCache;
        [NonSerialized] public Dictionary<int, HashSet<int>> zoneVisibility;
        [NonSerialized] public Dictionary<string, Waypoint> waypointCache;
        [NonSerialized] public Challenge challenge;
        [NonSerialized] public bool useExistingWorld;
        [NonSerialized] public int autoIdx = 1;
        [NonSerialized] public int livesLeft;
        [NonSerialized] public float tileSize;
        [NonSerialized] public float tileSizeY;
        [NonSerialized] public Vector2Int minSizeV = Vector2Int.zero;
        [NonSerialized] public Vector2Int maxSizeV = Vector2Int.zero;
        [NonSerialized] public bool pass2;

        public World()
        {
            chapters = new List<Chapter>();
            qualityRules = new List<QualityRule>();
            zones = new List<Zone>();
            zoneTemplates = new List<Zone>();
            spawnRules = new List<SpawnRule>();
            imageProviders = new List<ImageProvider>();
            replacements = new List<ReplacementRule>();
            objectSpecs = new List<ObjectSpec>();
            usedTags = new HashSet<string>();
            visitedZones = new Dictionary<string, int>();
            zoneTemplateCache = new Dictionary<string, string>();
            locationCache = new Dictionary<string, Tuple<string, BasicEntity>>();
            zoneVisibility = new Dictionary<int, HashSet<int>>();
            waypointCache = new Dictionary<string, Waypoint>();
            inventoryItems = new List<string>();
            speech = new List<TextFragment>();
            initialVariables = new List<Variable>();
            settings = new List<WorldSetting>();
            worldData = new List<WorldDataReference>();
            journeys = new List<Journey>();
            journeyTemplates = new List<Journey>();
            customShaders = new List<string>();
            dependencies = new WorldAnalysis();
        }

        public World(string key) : this()
        {
            this.key = key;
        }

        public void NullifyEmpties()
        {
            if (chapters is {Count: 0}) chapters = null;
            if (qualityRules is {Count: 0}) qualityRules = null;
            if (headItems is {Length: 0}) headItems = null;
            if (defaultRandomSkybox is {Length: 0}) defaultRandomSkybox = null;
            if (inventoryItems is {Count: 0}) inventoryItems = null;
            if (zones is {Count: 0}) zones = null;
            if (zoneTemplates is {Count: 0}) zoneTemplates = null;
            if (spawnRules is {Count: 0}) spawnRules = null;
            if (imageProviders is {Count: 0}) imageProviders = null;
            if (replacements is {Count: 0}) replacements = null;
            if (objectSpecs is {Count: 0}) objectSpecs = null;
            if (usedTags is {Count: 0}) usedTags = null;
            if (visitedZones is {Count: 0}) visitedZones = null;
            if (waypointCache is {Count: 0}) waypointCache = null;
            if (zoneTemplateCache is {Count: 0}) zoneTemplateCache = null;
            if (locationCache is {Count: 0}) locationCache = null;
            if (zoneVisibility is {Count: 0}) zoneVisibility = null;
            if (speech is {Count: 0}) speech = null;
            if (initialVariables is {Count: 0}) initialVariables = null;
            if (settings is {Count: 0}) settings = null;
            if (worldData is {Count: 0}) worldData = null;
            if (customShaders is {Count: 0}) customShaders = null;
            if (credits is {Count: 0}) credits = null;
            if (journeys is {Count: 0}) journeys = null;
            if (journeyTemplates is {Count: 0}) journeyTemplates = null;

            if (initialVariables != null)
            {
                foreach (Variable variable in initialVariables)
                {
                    if (variable.currentOrder is {Count: 0}) variable.currentOrder = null;
                    if (variable.targetOrder is {Count: 0}) variable.targetOrder = null;
                    if (variable.listeners is {Count: 0}) variable.listeners = null;
                }
            }
        }

        public override string ToString()
        {
            return $"World '{name}'";
        }
    }
}