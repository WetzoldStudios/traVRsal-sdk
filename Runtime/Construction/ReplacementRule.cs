using System;
using System.ComponentModel;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class ReplacementRule
    {
        public enum ReplacementType
        {
            Object,
            Material,
            Property
        }

        public ReplacementType type = ReplacementType.Object;
        public string key;
        public string objectKey;
        public string materials;
        public string[] randomMaterials;
        public string[] randomObjects;
        public string[] validZones;

        [DefaultValue(Direction.Same)]
        public Direction orientation = Direction.Same;

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

        public ReplacementRule WithType(ReplacementType type)
        {
            this.type = type;
            return this;
        }

        public override string ToString()
        {
            return $"Replacement Rule {key} ({type})";
        }
    }
}