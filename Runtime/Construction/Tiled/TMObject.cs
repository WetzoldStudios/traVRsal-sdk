using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class TMObject
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
        public string type;
        public bool visible;
        public float width;
        public float x;
        public float y;

        public override string ToString()
        {
            return $"Object {name} ({type})";
        }
    }
}