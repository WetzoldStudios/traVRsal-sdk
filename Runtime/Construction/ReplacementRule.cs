using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class ReplacementRule
    {
        public enum ReplacementType
        {
            OBJECT, MATERIAL
        }

        public string key;
        public ReplacementType type = ReplacementType.OBJECT;
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