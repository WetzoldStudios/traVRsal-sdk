using System;
using System.ComponentModel;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class Chapter
    {
        public enum UnlockRule
        {
            None = 0,
            EnterZone = 1,
            Variable = 3,
            Future = 2
        }

        public string key;
        public string name;
        public string description;
        public string coverImage;
        [DefaultValue(1)] public UnlockRule unlockRule = UnlockRule.EnterZone;
        public string condition;
        public string sku;
        public string unlockAchievement;
        [DefaultValue(99)] public Game.ReleaseChannel channelFilter = Game.ReleaseChannel.All;

        public Chapter()
        {
        }

        public Chapter(string key, UnlockRule unlockRule = UnlockRule.EnterZone) : this()
        {
            this.key = key;
            this.unlockRule = unlockRule;
        }

        public Chapter WithChannelFilter(Game.ReleaseChannel channelFilter)
        {
            this.channelFilter = channelFilter;

            return this;
        }

        public override string ToString()
        {
            return $"Chapter '{key}'";
        }
    }
}