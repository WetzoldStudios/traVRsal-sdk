using System;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class SegmentComponent
    {
        public const int UNLIMITED = -1;

        public enum ComponentType
        {
            Zone, Segment
        }

        [Header("Configuration")]
        public ComponentType type = ComponentType.Zone;
        public string key;
        [DefaultValue(1)]
        public int min = 1;
        [DefaultValue(UNLIMITED)]
        public int max = UNLIMITED;

        [Header("Runtime Information")]
        public string finalKey;
        public Zone zone;

        public SegmentComponent() { }

        public SegmentComponent(string key, int min = 1, int max = UNLIMITED) : this()
        {
            this.key = key;
            this.min = min;
            this.max = max;
        }

        public SegmentComponent(Zone zone, int min = 1, int max = UNLIMITED) : this(zone.name, min, max)
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
            return $"Component ({key})";
        }
    }
}