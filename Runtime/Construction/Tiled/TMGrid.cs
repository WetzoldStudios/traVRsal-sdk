using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TMGrid
    {
        public int height;
        public string orientation;
        public int width;

        public override string ToString()
        {
            return $"Grid ({width} x {height})";
        }
    }
}