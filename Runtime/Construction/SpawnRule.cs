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
            TOTAL, PERCENTAGE, PROBABILITY
        }

        public enum DistributionType
        {
            RANDOM, EVEN
        }

        public string key;
        public string layer;
        [DefaultValue(true)]
        public bool enabled = true;
        public string[] objectKeys;
        public string[] validZones;
        public string[] validSockets;
        public AmountType amountType = AmountType.TOTAL;
        [DefaultValue(10)]
        public int amount = 10;
        [DefaultValue(DistributionType.EVEN)]
        public DistributionType distributionType = DistributionType.EVEN;
        [DefaultValue(Direction.PATH_AHEAD)]
        public Direction orientation = Direction.PATH_AHEAD;
        public bool flipOrientation = false;
        public string[] restrictions;
        public int damage;
        public int health;
        [DefaultValue(100f)]
        public float scale = 100f;
        public float y = 0;
        public float[] yRange;
        public int minDistance = 0;
        public bool snap = false;
        public bool atCeiling = false;
        public bool connectToCeiling = false;
        public bool stretchToCeiling = false;
        public bool dontCountAsTarget = false;
        public bool dontCountAsObstacle = false;

        // FIXME: will cause "recursive serialization is not supported" warning
        public SpawnRule[] companions;

        // Formulas
        public string enabledFormula;

        public SpawnRule() { }

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
            return $"SpawnRule {key} ({amount} {amountType}, {distributionType})";
        }
    }
}