using System;
using System.ComponentModel;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class SpawnRule
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

        public string key;
        public string layer;
        [DefaultValue(true)] public bool enabled = true;
        public string[] objectKeys;
        public ConditionalValues[] objectKeysIf;

        public AmountType amountType = AmountType.Total;
        [DefaultValue(10)] public int amount = 10;
        [DefaultValue(DistributionType.Even)] public DistributionType distributionType = DistributionType.Even;
        [DefaultValue(Direction.Path_Ahead)] public Direction orientation = Direction.Path_Ahead;
        public SpaceRequirement occupy = SpaceRequirement.SingleSide;

        public float progressionStart;
        [DefaultValue(1.0f)] public float progressionEnd = 1f;

        public string[] validZones;
        public string[] validSockets;
        public string[] validObjects;
        public string[] restrictions;

        public int damage;
        public int health;

        public string scale;
        public float maxScale;
        public float y;
        public float[] yRange;
        public int minDistance;
        public string moveRandomly;
        public string rotateRandomly;
        public bool snap;
        public bool atCeiling;
        public bool connectToCeiling;
        public bool stretchToCeiling;
        public bool flipOrientation;

        public bool dontCountAsTarget;
        public bool dontCountAsObstacle;

        public bool debug;

        // FIXME: will cause "recursive serialization is not supported" warning
        public SpawnRule[] companions;

        // Formulas
        public string enabledFormula;

        public SpawnRule()
        {
        }

        public SpawnRule(string key) : this()
        {
            this.key = key;
        }

        public SpawnRule(string key, string[] objectKeys, int amount) : this()
        {
            this.key = key;
            this.objectKeys = objectKeys;
            this.amount = amount;
        }

        public bool IsSocketRule()
        {
            return validSockets != null && validSockets.Length > 0;
        }

        public SpawnRule WithValidSockets(string[] validSockets)
        {
            this.validSockets = validSockets;
            return this;
        }

        public SpawnRule WithValidObjects(string[] validObjects)
        {
            this.validObjects = validObjects;
            return this;
        }

        public SpawnRule WithValidZones(string[] validZones)
        {
            this.validZones = validZones;
            return this;
        }

        public SpawnRule WithRestrictions(string[] restrictions)
        {
            this.restrictions = restrictions;
            return this;
        }

        public SpawnRule WithDamage(int damage)
        {
            this.damage = damage;
            return this;
        }

        public SpawnRule WithAmount(int amount)
        {
            this.amount = amount;
            return this;
        }

        public SpawnRule WithAmountType(AmountType amountType)
        {
            this.amountType = amountType;
            return this;
        }

        public SpawnRule WithDistributionType(DistributionType distributionType)
        {
            this.distributionType = distributionType;
            return this;
        }

        public SpawnRule WithOrientation(Direction orientation)
        {
            this.orientation = orientation;
            return this;
        }

        public override string ToString()
        {
            return $"Spawn Rule {key} ({amount} {amountType}, {distributionType})";
        }
    }
}