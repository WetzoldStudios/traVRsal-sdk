using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class World
    {
        [Header("Configuration")]
        public string key;
        public string name;
        public string category = "Experience";
        public string shortDescription;
        public string longDescription;
        public string coverImage;
        public int availableTime = 5 * 60;
        public bool isVirtual = false;
        public bool enablePost = false;
        public string deathSound;
        public string kpis = "0,4"; // TODO: switch to array?
        public string detailKPIs = "0,4"; // TODO: switch to array?
        public string initialItemLeft;
        public string initialItemRight;
        public string introScenery = "/Base/LightHall";
        public string outroScenery = "/Base/LightHall-Outro";
        public MultiBehaviors behaviors;
        public List<Variable> initialVariables;
        public List<ObjectSpec> objectSpecs;
        public List<WorldSetting> settings;
        public List<SpawnRule> spawnRules;
        public List<ReplacementRule> replacements;
        public List<TextFragment> speech;
        public List<Credit> credits;
        public List<WorldDataReference> worldData;

        [Header("Runtime Data")]
        public List<string> dependencies;
        public List<string> worldDependencies;
        public List<Zone> zones;
        public List<Zone> zoneTemplates;

        [NonSerialized]
        public Dictionary<string, string> zoneTemplateCache;

        public World()
        {
            zones = new List<Zone>();
            zoneTemplates = new List<Zone>();
            spawnRules = new List<SpawnRule>();
            replacements = new List<ReplacementRule>();
            objectSpecs = new List<ObjectSpec>();
            zoneTemplateCache = new Dictionary<string, string>();
            speech = new List<TextFragment>();
            initialVariables = new List<Variable>();
            settings = new List<WorldSetting>();
            worldData = new List<WorldDataReference>();
            dependencies = new List<string>();
            worldDependencies = new List<string>();
        }

        public World(string key) : this()
        {
            this.key = key;
        }

        public override string ToString()
        {
            return $"World {name}";
        }
    }
}