using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class QualityRule
    {
        public string[] restrictions;

        public bool overrideRenderScale;
        public float renderScale;

        public bool overrideHQShaders;
        public bool hqShaders;

        public QualityRule()
        {
        }

        public override string ToString()
        {
            return "Quality Rule";
        }
    }
}