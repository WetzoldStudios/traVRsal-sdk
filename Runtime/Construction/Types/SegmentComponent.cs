using System;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class SegmentComponent
    {
        public const int Unlimited = -1;

        public enum ComponentType
        {
            Zone, Segment
        }

        [Header("Configuration")]
        public ComponentType type = ComponentType.Zone;
        public string key;
        [DefaultValue(1)]
        public int min = 1;
        [DefaultValue(Unlimited)]
        public int max = Unlimited;

        [Header("Runtime Information")]
        public string finalKey;
        public Zone zone;

        public SegmentComponent() { }

        public SegmentComponent(string key, int min = 1, int max = Unlimited) : this()
        {
            this.key = key;
            this.min = min;
            this.max = max;
        }

        public SegmentComponent(Zone zone, int min = 1, int max = Unlimited) : this(zone.name, min, max)
        {
            this.zone = zone;
        }

        public SegmentComponent WithKey(string key)
        {
            this.key = key;

            return this;
        }

        public SegmentComponent WithType(ComponentType type)
        {
            this.type = type;

            return this;
        }

        public override string ToString()
        {
            return $"Component '{key}'";
        }
    }
}