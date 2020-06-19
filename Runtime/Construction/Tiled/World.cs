using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class World
    {
        public string type;
        public WorldMap[] maps;
        public TMProperty[] properties; // custom attribute, not in format definition yet but raised in forum

        // automatically added by importer
        public string fileName;
        public string worldName;

        public override string ToString()
        {
            return $"World {worldName} ({fileName})";
        }
    }
}