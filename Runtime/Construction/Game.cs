using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Game
    {
        [Header("Configuration")]
        public string key = "game";
        public string name = "Development Mode";
        public string worldsURL;
        public string userWorldsURL;
        public List<string> worlds;
        public string menuScene = "Menu-OoD";
        public string worldObject = "World2";
        public string pauseScene = "Pause-OoD";
        public bool devMode = false;

        public Game()
        {
            worlds = new List<string>();
        }

        public override string ToString()
        {
            return $"Game {name} ({key}, {worlds.Count} worlds, " + (devMode ? "Dev-Mode" : "Game-Mode") + ")";
        }
    }
}