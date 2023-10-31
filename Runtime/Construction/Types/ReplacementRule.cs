using System;
using System.ComponentModel;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class ReplacementRule
    {
        public enum ReplacementType
        {
            Object = 0,
            Material = 1,
            Property = 2
        }

        public ReplacementType type = ReplacementType.Object;
        public string key;
        public string[] keys;

        public string objectKey;
        public string[] randomObjects;
        [DefaultValue(Direction.Same)] public Direction orientation = Direction.Same;

        public string materials;
        public string[] randomMaterials;

        public string[] validZones;
        public string[] invalidZones;
        public string[] validTags;

        public TMProperty[] properties;

        public ReplacementRule()
        {
        }

        public ReplacementRule(string key, string objectKey, string materials = null) : this()
        {
            this.key = key;
            this.objectKey = objectKey;
            this.materials = materials;
        }

        public ReplacementRule(ReplacementRule copyFrom) : this()
        {
            type = copyFrom.type;
            key = copyFrom.key;
            keys = copyFrom.keys;
            objectKey = copyFrom.objectKey;
            materials = copyFrom.materials;
            randomMaterials = copyFrom.randomMaterials;
            randomObjects = copyFrom.randomObjects;
            validZones = copyFrom.validZones;
            invalidZones = copyFrom.invalidZones;
            validTags = copyFrom.validTags;
            orientation = copyFrom.orientation;
            properties = SDKUtil.CopyProperties(copyFrom.properties);
        }

        public ReplacementRule WithType(ReplacementType type)
        {
            this.type = type;
            return this;
        }

        public override string ToString()
        {
            return $"Replacement Rule '{key}' ({type})";
        }
    }
}