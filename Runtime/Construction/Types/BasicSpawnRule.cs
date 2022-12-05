using System;
using System.ComponentModel;
using UnityEngine;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class BasicSpawnRule
    {
        public enum AmountType
        {
            Total = 0,
            Probability = 1
        }

        public enum DistributionType
        {
            Random = 0,
            Even = 1,
            TowardsEnd = 3,
            Object = 2
        }

        public enum SpaceRequirement
        {
            SingleSide = 0,
            AllSides = 1,
            All = 2
        }

        [Header("General")] public string key;
        public string layer;

        [Header("Objects")] public string[] objectKeys;
        public ConditionalValues[] objectKeysIf;

        [Header("Distribution")] public AmountType amountType = AmountType.Total;
        [DefaultValue(10)] public int amount = 10;
        [DefaultValue(DistributionType.Even)] public DistributionType distributionType = DistributionType.Even;
        [DefaultValue(Direction.Path_Ahead)] public Direction orientation = Direction.Path_Ahead;
        public SpaceRequirement occupy = SpaceRequirement.SingleSide;

        public float progressionStart;
        [DefaultValue(1.0f)] public float progressionEnd = 1f;

        [Header("Restrictions")] public bool currentZone;
        public bool exactLocation;
        public string[] validZones;
        public string[] validTags;
        public string[] validSockets;
        public string[] validObjects;
        public string[] restrictions;
        public int minDistance;

        [Header("Positioning")] public float y;
        public float[] yRange;
        public string moveRandomly;
        public string rotateRandomly;
        public string scale;
        public float maxScale;
        public bool snap;
        public bool atCeiling;
        public bool connectToCeiling;
        public bool stretchToCeiling;
        public bool flipOrientation;
        public bool considerPlayerHeight;

        [Header("Property Overrides")] public int damage;
        public int health;
        [DefaultValue(-1.0f)] public float speed = -1f;
        public string waypoints;

        [Header("Shields")] [DefaultValue(-1.0f)]
        public float shieldProbability = -1f;

        [DefaultValue(-1.0f)] public float shieldRegenerationPeriod = -1f;
        [DefaultValue(-1)] public int minShields = -1;
        [DefaultValue(-1)] public int maxShields = -1;
        [DefaultValue(-1)] public int maxShieldStrength = -1;

        [Header("Misc")] [DefaultValue(true)] public bool enabled = true;
        public string enabledFormula;
        public bool dontCountAsTarget;
        public bool dontCountAsObstacle;
        public bool debug;

        public BasicSpawnRule()
        {
        }

        public BasicSpawnRule(string key) : this()
        {
            this.key = key;
        }

        public BasicSpawnRule(string key, string[] objectKeys, int amount) : this()
        {
            this.key = key;
            this.objectKeys = objectKeys;
            this.amount = amount;
        }

        public bool IsSocketRule()
        {
            return validSockets != null && validSockets.Length > 0;
        }

        public BasicSpawnRule WithValidSockets(string[] validSockets)
        {
            this.validSockets = validSockets;
            return this;
        }

        public BasicSpawnRule WithValidObjects(string[] validObjects)
        {
            this.validObjects = validObjects;
            return this;
        }

        public BasicSpawnRule WithValidZones(string[] validZones)
        {
            this.validZones = validZones;
            return this;
        }

        public BasicSpawnRule WithRestrictions(string[] restrictions)
        {
            this.restrictions = restrictions;
            return this;
        }

        public BasicSpawnRule WithDamage(int damage)
        {
            this.damage = damage;
            return this;
        }

        public BasicSpawnRule WithAmount(int amount)
        {
            this.amount = amount;
            return this;
        }

        public BasicSpawnRule WithAmountType(AmountType amountType)
        {
            this.amountType = amountType;
            return this;
        }

        public BasicSpawnRule WithDistributionType(DistributionType distributionType)
        {
            this.distributionType = distributionType;
            return this;
        }

        public BasicSpawnRule WithOrientation(Direction orientation)
        {
            this.orientation = orientation;
            return this;
        }

        public override string ToString()
        {
            return $"Simple Spawn Rule {key} ({amount} {amountType}, {distributionType})";
        }
    }
}