using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class ReplacementRule
    {
        public enum ReplacementType
        {
            Object, Material
        }

        public string key;
        public ReplacementType type = ReplacementType.Object;
        public string objectKey;
        public string materials;

        public ReplacementRule() { }

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
            return $"ReplacementRule {key} ({type})";
        }
    }
}