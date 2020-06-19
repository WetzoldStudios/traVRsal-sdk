using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldMap
    {
        public string fileName;
        public int x;
        public int y;

        public override string ToString()
        {
            return $"WorldMap {fileName}";
        }
    }
}