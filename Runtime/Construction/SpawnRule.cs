using System;
using System.ComponentModel;
using UnityEngine;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    public class SpawnRule : BasicSpawnRule
    {
        [Tooltip("Other game objects that are spawned at the exact same position the main spawn rule determined, e.g. a frame for a picture")]
        public BasicSpawnRule[] companions;

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

        public new SpawnRule WithValidSockets(string[] validSockets)
        {
            this.validSockets = validSockets;
            return this;
        }

        public new SpawnRule WithValidObjects(string[] validObjects)
        {
            this.validObjects = validObjects;
            return this;
        }

        public new SpawnRule WithValidZones(string[] validZones)
        {
            this.validZones = validZones;
            return this;
        }

        public new SpawnRule WithRestrictions(string[] restrictions)
        {
            this.restrictions = restrictions;
            return this;
        }

        public new SpawnRule WithDamage(int damage)
        {
            this.damage = damage;
            return this;
        }

        public new SpawnRule WithAmount(int amount)
        {
            this.amount = amount;
            return this;
        }

        public new SpawnRule WithAmountType(AmountType amountType)
        {
            this.amountType = amountType;
            return this;
        }

        public new SpawnRule WithDistributionType(DistributionType distributionType)
        {
            this.distributionType = distributionType;
            return this;
        }

        public new SpawnRule WithOrientation(Direction orientation)
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