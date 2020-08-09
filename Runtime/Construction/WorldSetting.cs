using System;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldSetting
    {
        public enum SettingsType
        {
            BOOLEAN
        }

        public string key;
        [DefaultValue("true")]
        public string defaultValue = "true";
        public string name;
        public string description;
        public SettingsType type = SettingsType.BOOLEAN;

        [Header("Runtime Data")]
        public string currentValue;

        public WorldSetting() { }

        public WorldSetting(string key) : this()
        {
            this.key = key;
        }

        public string GetCurrentValue()
        {
            return string.IsNullOrEmpty(currentValue) ? defaultValue : currentValue;
        }

        public WorldSetting WithCurrentValue(string currentValue)
        {
            this.currentValue = currentValue;
            return this;
        }

        public WorldSetting WithDefaultValue(string defaultValue)
        {
            this.defaultValue = defaultValue;
            return this;
        }

        public override string ToString()
        {
            return $"WorldSetting {key} ({type})";
        }
    }
}