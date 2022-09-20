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
        public string type;

        public override string ToString()
        {
            return $"Tile '{id}' ({type})";
        }
    }
}