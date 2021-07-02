using Bhaptics.Tact.Unity;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Damage Inflictor")]
    public class DamageInflictor : MonoBehaviour
    {
        public enum DamageSource
        {
            Enemy = 0,
            Player = 1,
            Environment = 2,
            Boundary = 3,
            Falling = 4,
            Timer = 5
        }

        [Header("Configuration")] public bool instantKill;

        [Tooltip("Hit points to be deducted from player health")]
        public int damage = 1;

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
        public GameObject hitAnimation; // only relevant for bullets right now

        // runtime 
        [HideInInspector] public string originTag;
        private float lastHit;

        private void Start()
        {
            if (string.IsNullOrEmpty(originTag)) originTag = gameObject.tag;
        }

        public bool IsActive()
        {
            return enabled && Time.time > lastHit + cooldown;
        }

        public void RegisterHit()
        {
            lastHit = Time.time;
        }
    }
}