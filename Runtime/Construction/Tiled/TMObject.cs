using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TMObject
    {
        public bool ellipse;
        public uint gid;
        public float height;
        public int id;
        public string name;
        public bool point;
        public TMProperty[] properties;
        public float rotation;
        public string template;
        public string @class;
        public string type;
        public bool visible;
        public float width;
        public float x;
        public float y;

        public void Init()
        {
            // compatibility with class introduction from Tiled 1.9+
            type = string.IsNullOrWhiteSpace(type) ? @class : type;
        }

        public override string ToString()
        {
            return $"Object '{name}' ({type})";
        }
    }
}