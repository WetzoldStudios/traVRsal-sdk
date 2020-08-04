using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldDataReference
    {
        public enum ImportType
        {
            TILEMAP, TILEMAP_WORLD, WORLD
        }

        public ImportType type = ImportType.TILEMAP_WORLD;
        public string fileName;

        public WorldDataReference() { }

        public WorldDataReference(string fileName, ImportType type = ImportType.TILEMAP_WORLD) : this()
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