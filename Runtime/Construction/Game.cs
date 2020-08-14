using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Game
    {
        [Header("Configuration")]
        [DefaultValue("game")]
        public string key = "game";
        [DefaultValue("Development Mode")]
        public string name = "Development Mode";
        public string worldsURL;
        public string userWorldsURL;
        public List<string> worlds;
        [DefaultValue("Menu-traVRsal")]
        public string menuScene = "Menu-traVRsal";
        [DefaultValue("/Base/Menu/Level2")]
        public string worldObject = "/Base/Menu/Level2";
        [DefaultValue("Pause-OoD")]
        public string pauseScene = "Pause-OoD";
        public bool devMode = false;

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
            return $"Game {name} ({key}, {worlds.Count} worlds, " + (devMode ? "Dev-Mode" : "Game-Mode") + ")";
        }
    }
}