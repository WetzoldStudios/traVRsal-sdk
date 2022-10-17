using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TMTile
    {
        public int id;
        public string image;
        public int imageheight;
        public int imagewidth;
        public TMProperty[] properties;
        public string @class;
        public string type;

        public void Init()
        {
            // compatibility with class introduction from Tiled 1.9+
            type = string.IsNullOrWhiteSpace(type) ? @class : type;
        }

        public override string ToString()
        {
            return $"Tile '{id}' ({type})";
        }
    }
}