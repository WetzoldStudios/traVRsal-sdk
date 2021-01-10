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
            Enemy = 0,
            Environment = 1,
            PlayerHead = 2,
            PlayerLeftArm = 3,
            PlayerRightArm = 4,
            PlayerBody = 5,
            Player = 6
        }

        public enum DestroyAction
        {
            None,
            Move,
            Rotate
        }

        [Header("Configuration")] public ObjectType type = ObjectType.Enemy;
        public int health = 1;
        public float damageMultiplier = 1f;
        public int points;
        public bool isPlayer;
        public bool destructible = true;
        public bool registerAsTarget = true;
        public bool triggerVariable;

        [Header("Destruction")] public bool allowMelee;
        public bool hideWhenDestroyed = true;
        public string stateChange;

        [Header("Destruction Effects")] public DestroyAction destroyAction = DestroyAction.None;
        public Vector3 endValue;
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
            triggerVariable = copyFrom.triggerVariable;
            allowMelee = copyFrom.allowMelee;
            hideWhenDestroyed = copyFrom.hideWhenDestroyed;
            stateChange = copyFrom.stateChange;
            destroyAction = copyFrom.destroyAction;
            endValue = copyFrom.endValue;
            duration = copyFrom.duration;
            breakSounds = copyFrom.breakSounds;
        }
    }
}