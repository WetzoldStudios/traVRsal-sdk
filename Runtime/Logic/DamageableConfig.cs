﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class DamageableConfig : BehaviorConfig
    {
        public enum DestroyAction
        {
            None,
            Move,
            Rotate
        }

        [Header("Configuration")] public int health = 1;
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
            health = copyFrom.health;
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