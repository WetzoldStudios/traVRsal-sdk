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
        public List<string> levels;
        public bool devMode = false;

        public Game()
        {
            levels = new List<string>();
        }

        public override string ToString()
        {
            return $"Game {name} ({key}, {levels.Count} levels, " + (devMode ? "Dev-Mode" : "Game-Mode") + ")";
        }
    }
}