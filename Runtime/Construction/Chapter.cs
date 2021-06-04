using System;
using System.ComponentModel;

namespace traVRsal.SDK
{
    [Serializable]
    public class Chapter
    {
        public enum UnlockRule
        {
            None = 0,
            EnterZone = 1,
            Future = 2
        }

        public string key;
        public string name;
        public string description;
        public string coverImage;
        [DefaultValue(1)] public UnlockRule unlockRule = UnlockRule.EnterZone;

        public Chapter()
        {
        }

        public Chapter(string key, UnlockRule unlockRule = UnlockRule.EnterZone) : this()
        {
            this.key = key;
            this.unlockRule = unlockRule;
        }

        public override string ToString()
        {
            return $"Chapter {key}";
        }
    }
}