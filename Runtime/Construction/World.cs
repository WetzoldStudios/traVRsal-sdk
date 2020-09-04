using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class World
    {
        [Header("Configuration")]
        public string key;
        public string name;
        [DefaultValue("Experience")]
        public string category = "Experience";
        public string shortDescription;
        public string longDescription;
        public string coverImage;
        [DefaultValue(300)]
        public int availableTime = 5 * 60;
        public bool isVirtual = false;
        public bool enablePost = false;
        public string deathSound;
        [DefaultValue("0,4")]
        public string kpis = "0,4"; // TODO: switch to array?
        [DefaultValue("0,4")]
        public string detailKPIs = "0,4"; // TODO: switch to array?
        public string initialItemLeft;
        public string initialItemRight;
        [DefaultValue("/Base/LightHall")]
        public string introScenery = "/Base/LightHall";
        [DefaultValue("/Base/LightHall-Outro")]
        public string outroScenery = "/Base/LightHall-Outro";
        public List<WorldDataReference> worldData;
        public List<Journey> journeys;
        public List<WorldSetting> settings;
        public List<ImageProvider> imageProviders;
        public List<Variable> initialVariables;
        public List<TextFragment> speech;
        public List<ReplacementRule> replacements;
        public List<SpawnRule> spawnRules;
        public MultiBehaviors behaviors;
        public List<Credit> credits;

        [Header("Runtime Data")]
        public bool journeyMode = false;
        public List<ObjectSpec> objectSpecs;
        public List<string> dependencies;
        public List<string> worldDependencies;
        public List<Zone> zones;
        public List<Zone> zoneTemplates;

        [NonSerialized]
        public Dictionary<string, string> zoneTemplateCache;
        [NonSerialized]
        public UserWorld remoteMetaData;
        [NonSerialized]
        public int autoIdx = 1;

        public World()
        {
            zones = new List<Zone>();
            zoneTemplates = new List<Zone>();
            spawnRules = new List<SpawnRule>();
            imageProviders = new List<ImageProvider>();
            replacements = new List<ReplacementRule>();
            objectSpecs = new List<ObjectSpec>();
            zoneTemplateCache = new Dictionary<string, string>();
            speech = new List<TextFragment>();
            initialVariables = new List<Variable>();
            settings = new List<WorldSetting>();
            worldData = new List<WorldDataReference>();
            journeys = new List<Journey>();
            dependencies = new List<string>();
            worldDependencies = new List<string>();
        }

        public World(string key) : this()
        {
            this.key = key;
        }

        public void NullifyEmpties()
        {
            if (zones != null && zones.Count == 0) zones = null;
            if (zoneTemplates != null && zoneTemplates.Count == 0) zoneTemplates = null;
            if (spawnRules != null && spawnRules.Count == 0) spawnRules = null;
            if (imageProviders != null && imageProviders.Count == 0) imageProviders = null;
            if (replacements != null && replacements.Count == 0) replacements = null;
            if (objectSpecs != null && objectSpecs.Count == 0) objectSpecs = null;
            if (zoneTemplateCache != null && zoneTemplateCache.Count == 0) zoneTemplateCache = null;
            if (speech != null && speech.Count == 0) speech = null;
            if (initialVariables != null && initialVariables.Count == 0) initialVariables = null;
            if (settings != null && settings.Count == 0) settings = null;
            if (worldData != null && worldData.Count == 0) worldData = null;
            if (dependencies != null && dependencies.Count == 0) dependencies = null;
            if (worldDependencies != null && worldDependencies.Count == 0) worldDependencies = null;
            if (credits != null && credits.Count == 0) credits = null;
            if (journeys != null && journeys.Count == 0) journeys = null;

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