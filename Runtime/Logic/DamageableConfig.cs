using System;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
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
        public float health = 1;
        public float damageMultiplier = 1f;
        public float regeneratePerSecond;
        public int points;
        public bool isPlayer;
        public bool registerAsTarget = true;
        public bool registerAsTargetIfDisabled;
        public string targetName;
        public bool triggerVariable;
        public string variable;
        [Range(0, 5)] public int variableChannel;

        [Header("Destruction")] public bool destructible = true;
        public bool allowMelee;
        public bool hideWhenDestroyed = true;
        public string stateChange;

        [Tooltip("Haptics to play, e.g. 'melee'")]
        public string haptics;

        public HapticClip[] customHaptics;
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
            regeneratePerSecond = copyFrom.regeneratePerSecond;
            points = copyFrom.points;
            isPlayer = copyFrom.isPlayer;
            destructible = copyFrom.destructible;
            registerAsTarget = copyFrom.registerAsTarget;
            registerAsTargetIfDisabled = copyFrom.registerAsTargetIfDisabled;
            targetName = copyFrom.targetName;
            triggerVariable = copyFrom.triggerVariable;
            variable = copyFrom.variable;
            allowMelee = copyFrom.allowMelee;
            hideWhenDestroyed = copyFrom.hideWhenDestroyed;
            stateChange = copyFrom.stateChange;
            haptics = copyFrom.haptics;
            duration = copyFrom.duration;
            breakSounds = copyFrom.breakSounds;
        }

        public override string ToString()
        {
            return $"Damageable Config ({type})";
        }
    }
}