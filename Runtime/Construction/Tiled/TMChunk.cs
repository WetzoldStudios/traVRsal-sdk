using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class TMChunk
    {
        public uint[] data;
        public int height;
        public int width;
        public int x;
        public int y;

        public override string ToString()
        {
            return $"Chunk ({width} x {height})";
        }
    }
}