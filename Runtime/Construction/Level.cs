using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Level
    {
        [Header("Configuration")]
        public string key;
        public string name;
        public string category = "Experience";
        public string shortDescription;
        public string longDescription;
        public string coverImage;
        public int availableTime = 5 * 60;
        public bool enablePost = false;
        public string deathSound;
        public string kpis = "0,4"; // TODO: switch to array?
        public string detailKPIs = "0,4"; // TODO: switch to array?
        public string initialItemLeft;
        public string initialItemRight;
        public MultiBehaviors behaviors;
        public List<Variable> initialVariables;
        public List<ObjectSpec> objectSpecs;
        public List<LevelSetting> settings;
        public List<SpawnRule> spawnRules;
        public List<ReplacementRule> replacements;
        public List<TextFragment> speech;
        public List<Credit> credits;
        public List<LevelDataReference> levelData;

        [Header("Runtime Data")]
        public List<string> dependencies;
        public List<string> levelDependencies;
        public List<Room> rooms;
        public List<Room> roomTemplates;

        [NonSerialized]
        public Dictionary<string, string> roomTemplateCache;

        public Level()
        {
            rooms = new List<Room>();
            roomTemplates = new List<Room>();
            spawnRules = new List<SpawnRule>();
            replacements = new List<ReplacementRule>();
            objectSpecs = new List<ObjectSpec>();
            roomTemplateCache = new Dictionary<string, string>();
            speech = new List<TextFragment>();
            initialVariables = new List<Variable>();
            settings = new List<LevelSetting>();
            levelData = new List<LevelDataReference>();
            dependencies = new List<string>();
            levelDependencies = new List<string>();
        }

        public Level(string key) : this()
        {
            this.key = key;
        }

        public override string ToString()
        {
            return $"Level {name}";
        }
    }
}