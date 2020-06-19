using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class LevelDataReference
    {
        public enum ImportType
        {
            TILEMAP, TILEMAP_WORLD, LEVEL
        }

        public ImportType type = ImportType.TILEMAP_WORLD;
        public string fileName;

        public LevelDataReference() { }

        public LevelDataReference(string fileName, ImportType type = ImportType.TILEMAP_WORLD) : this()
        {
            this.fileName = fileName;
            this.type = type;
        }

        public override string ToString()
        {
            return $"LevelDataReference {fileName} ({type})";
        }
    }
}