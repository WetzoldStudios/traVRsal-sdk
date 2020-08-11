using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldDataReference
    {
        public enum ImportType
        {
            TileMap, TileMap_World, World
        }

        public ImportType type = ImportType.TileMap_World;
        public string fileName;

        public WorldDataReference() { }

        public WorldDataReference(string fileName, ImportType type = ImportType.TileMap_World) : this()
        {
            this.fileName = fileName;
            this.type = type;
        }

        public override string ToString()
        {
            return $"WorldDataReference {fileName} ({type})";
        }
    }
}