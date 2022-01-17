using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;
using static traVRsal.SDK.Shield;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Damage Inflictor")]
    public class DamageInflictor : MonoBehaviour
    {
        public enum DamageSource
        {
            Enemy = 0,
            Player = 1,
            OtherPlayer = 6,
            Environment = 2,
            Boundary = 3,
            Falling = 4,
            Timer = 5
        }

        [Header("Configuration")] public bool instantKill;

        [Tooltip("Hit points to be deducted from player health")]
        public float damage = 1;

        [Tooltip("Type of shield the weapon can damage")]
        public ShieldType shieldDamage = ShieldType.None;

        [Tooltip("Indicator if damage is inflicted through a melee weapon")]
        public bool melee;

        public DamageSource source;
        public bool killAfterAttack;

        [Tooltip("Only relevant for projectiles right now")]
        public float lifeTime = 5f;

        [Tooltip("Time span in which no further damage is inflicted after initial hit")]
        public float cooldown = 2f;

        [Header("When Dealing Damage")] public string haptics;
        public HapticClip[] customHaptics;
        public bool localizedHaptics;
        public GameObject hitAnimation; // only relevant for bullets right now

        // runtime 
        [HideInInspector] public string originTag;
        private Dictionary<GameObject, float> _lastHit;
        private bool _initDone;

        private void Start()
        {
            if (string.IsNullOrEmpty(originTag)) originTag = gameObject.tag;

            Init();
        }

        private void Init()
        {
            if (_initDone) return;
            _initDone = true;
            _lastHit = new Dictionary<GameObject, float>();
        }

        public bool IsActive(GameObject go)
        {
            if (!enabled) return false;
            if (!_lastHit.ContainsKey(go)) return true;

            return Time.time > _lastHit[go] + cooldown;
        }

        public void RegisterHit(GameObject go)
        {
            Init();

            if (!_lastHit.ContainsKey(go))
            {
                _lastHit.Add(go, Time.time);
            }
            else
            {
                _lastHit[go] = Time.time;
            }
        }
    }
}