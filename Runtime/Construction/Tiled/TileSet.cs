using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TileSet
    {
        public int columns;
        public int firstgid;
        public TMGrid grid;
        public string image;
        public int imagewidth;
        public int imageheight;
        public int margin;
        public string name;
        public TMProperty[] properties;
        public int spacing;
        public int tilecount;
        public int tileheight;
        public TMTile[] tiles;
        public int tilewidth;
        public string transparentcolor;
        public string type;

        public override string ToString()
        {
            return $"TileSet {name} ({type})";
        }
    }
}