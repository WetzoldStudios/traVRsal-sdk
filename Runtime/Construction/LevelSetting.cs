using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class LevelSetting
    {
        public enum SettingsType
        {
            BOOLEAN
        }

        public string key;
        public string defaultValue = "true";
        public string name;
        public string description;
        public SettingsType type = SettingsType.BOOLEAN;

        [Header("Runtime Data")]
        public string currentValue;

        public LevelSetting() { }

        public LevelSetting(string key) : this()
        {
            this.key = key;
        }

        public string GetCurrentValue()
        {
            return string.IsNullOrEmpty(currentValue) ? defaultValue : currentValue;
        }

        public LevelSetting WithCurrentValue(string currentValue)
        {
            this.currentValue = currentValue;
            return this;
        }

        public LevelSetting WithDefaultValue(string defaultValue)
        {
            this.defaultValue = defaultValue;
            return this;
        }

        public override string ToString()
        {
            return $"LevelSetting {key} ({type})";
        }
    }
}