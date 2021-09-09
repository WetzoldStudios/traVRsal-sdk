using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Shield
    {
        public enum ShieldType
        {
            None = 0,
            Orange = 1,
            Blue = 2,
            Any = 3
        }

        [Header("Configuration")] public Transform anchor;
        public int strength;
        public int maxStrength = 5;

        [Tooltip("Time in seconds until shield strength is increased by 1. A value of 0 deactivates regeneration.")]
        public float regenerationPeriod;
    }
}