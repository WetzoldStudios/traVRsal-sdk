using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldMap
    {
        public string fileName;
        public int x;
        public int y;

        public WorldMap()
        {
        }

        public WorldMap(string fileName) : this()
        {
            this.fileName = fileName;
        }

        public override string ToString()
        {
            return $"World Map {fileName}";
        }
    }
}