using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Game
    {
        public enum ReleaseChannel
        {
            Live = 0,
            Beta = 1,
            Alpha = 2
        }

        public enum PublishingEnvironment
        {
            Other = 0,
            Oculus = 1
        }

        [Header("Configuration")] [DefaultValue("game")]
        public string key = "game";

        public ReleaseChannel channel = ReleaseChannel.Live;
        public PublishingEnvironment environment = PublishingEnvironment.Other;

        [DefaultValue("Development Mode")] public string name = "Development Mode";
        public List<string> worlds;
        [DefaultValue("Intro")] public string introWorld = "Intro";
        [DefaultValue("Menu-traVRsal")] public string menuScene = "Menu-traVRsal";
        [DefaultValue("Pause-traVRsal")] public string pauseScene = "Pause-traVRsal";
        [DefaultValue("GameOver-traVRsal")] public string gameOverScene = "GameOver-traVRsal";
        [DefaultValue("Theater-traVRsal")] public string theaterScene = "Theater-traVRsal";
        public bool devMode;
        public bool demoMode;
        public bool enableMultiplayer;

        // derived 
        public bool offlineMode;

        public Game()
        {
            worlds = new List<string>();
        }

        public void NullifyEmpties()
        {
            if (worlds != null && worlds.Count == 0) worlds = null;
        }

        public override string ToString()
        {
            return $"Game '{name}' ({key}, {worlds.Count} worlds, " + (devMode ? "Dev-Mode" : "Game-Mode") + ")";
        }
    }
}