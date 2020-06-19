using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class TileMap
    {
        public string backgroundcolor;
        public int compressionlevel;
        public int height;
        public int hexsidelength;
        public bool infinite;
        public TMLayer[] layers;
        public int nextlayerid;
        public int nextobjectid;
        public string orientation;
        public TMProperty[] properties;
        public string renderorder;
        public string staggeraxis;
        public string staggerindex;
        public string tiledversion;
        public int tileheight;
        public TileSet[] tilesets;
        public int tilewidth;
        public string type;
        public float version;
        public int width;

        // automatically added by importer
        public string fileName;
        public string mapName;

        public TMTile GetTileByGid(uint gid)
        {
            foreach (TileSet tileSet in tilesets)
            {
                // go through all IDs since they can be offset and don't correspond to their index which also does not allow to calculate this up-front
                if (tileSet != null && tileSet.tiles != null)
                {
                    foreach (TMTile tile in tileSet.tiles)
                    {
                        if (tile.id + tileSet.firstgid == gid) return tile;
                    }
                }
                else
                {
                    EDebug.LogError($"Referenced TileSet could not be found: {fileName}");
                    return null;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return $"TileMap {mapName} ({fileName})";
        }
    }
}