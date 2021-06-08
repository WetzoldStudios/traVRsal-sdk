using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class DamageableConfig : BehaviorConfig
    {
        public enum ObjectType
        {
            EnemyOrCollectible = 0,
            Environment = 1,
            PlayerHead = 2,
            PlayerLeftArm = 3,
            PlayerRightArm = 4,
            PlayerBody = 5,
            Player = 6
        }

        [Header("Configuration")] public ObjectType type = ObjectType.EnemyOrCollectible;
        public int health = 1;
        public float damageMultiplier = 1f;
        public int points;
        public bool isPlayer;
        public bool registerAsTarget = true;
        public string targetName;
        public bool triggerVariable;
        public string variable;

        [Header("Destruction")] public bool destructible = true;
        public bool allowMelee;
        public bool hideWhenDestroyed = true;
        public string stateChange;
        public float duration = 0.2f;

        [HideInInspector] public List<string> breakSounds;

        public DamageableConfig()
        {
        }

        public DamageableConfig(DamageableConfig copyFrom)
        {
            type = copyFrom.type;
            health = copyFrom.health;
            damageMultiplier = copyFrom.damageMultiplier;
            points = copyFrom.points;
            isPlayer = copyFrom.isPlayer;
            destructible = copyFrom.destructible;
            registerAsTarget = copyFrom.registerAsTarget;
            targetName = copyFrom.targetName;
            triggerVariable = copyFrom.triggerVariable;
            variable = copyFrom.variable;
            allowMelee = copyFrom.allowMelee;
            hideWhenDestroyed = copyFrom.hideWhenDestroyed;
            stateChange = copyFrom.stateChange;
            duration = copyFrom.duration;
            breakSounds = copyFrom.breakSounds;
        }

        public override string ToString()
        {
            return $"Damageable Config ({type})";
        }
    }
}