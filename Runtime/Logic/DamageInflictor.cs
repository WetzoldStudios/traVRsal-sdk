using Bhaptics.Tact.Unity;
using UnityEngine;

namespace traVRsal.SDK
{
    public class DamageInflictor : MonoBehaviour
    {
        public enum DamageSource
        {
            Enemy = 0,
            Player = 1,
            Environment = 2,
            Boundary = 3
        }

        [Header("Configuration")] public bool instantKill;
        public int damage = 1;
        public bool melee;
        public DamageSource source;
        public bool killAfterAttack;
        public float lifeTime = 5f; // only relevant for bullets right now
        public float cooldown = 2f;

        [Header("When Dealing Damage")] public string haptics;
        public HapticClip[] customHaptics;
        public GameObject hitAnimation; // only relevant for bullets right now

        [Header("Runtime")] public string originTag;
        public float lastHit;

        private void Start()
        {
            if (string.IsNullOrEmpty(originTag)) originTag = gameObject.tag;
        }

        public bool IsActive()
        {
            return enabled && Time.time > lastHit + cooldown;
        }
    }
}