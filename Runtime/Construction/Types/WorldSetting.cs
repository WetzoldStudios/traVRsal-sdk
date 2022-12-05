using System;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class WorldSetting
    {
        public enum SettingsType
        {
            Boolean = 0,
            ImageProvider = 1,
            Link = 2
        }

        public string key;
        [DefaultValue("true")] public string defaultValue = "true";
        public string name;
        public string description;
        public SettingsType type = SettingsType.Boolean;

        [Header("Runtime Data")] public string currentValue;

        public WorldSetting()
        {
        }

        public WorldSetting(string key) : this()
        {
            this.key = key;
        }

        public string GetCurrentValue()
        {
            return string.IsNullOrEmpty(currentValue) ? defaultValue : currentValue;
        }

        public WorldSetting WithCurrentValue(string value)
        {
            currentValue = value;
            return this;
        }

        public WorldSetting WithDefaultValue(string value)
        {
            defaultValue = value;
            return this;
        }

        public override string ToString()
        {
            return $"World Setting {key} ({type})";
        }
    }
}