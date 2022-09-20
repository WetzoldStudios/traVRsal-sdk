using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TMLayer
    {
        public TMChunk[] chunks;
        public string compression;
        public uint[] data;
        public string draworder;
        public string encoding;
        public int height;
        public int id;
        public string image;
        public TMLayer[] layers;
        public string name;
        public TMObject[] objects;
        public float offsetx;
        public float offsety;
        public float opacity;
        public TMProperty[] properties;
        public string transparentcolor;
        public string type;
        public bool visible;
        public int width;
        public int x;
        public int y;

        public override string ToString()
        {
            return $"Layer '{name}' ({type})";
        }
    }
}