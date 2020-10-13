using UnityEngine;

namespace traVRsal.SDK
{
    public class DamageInflictor : MonoBehaviour
    {
        [Header("Configuration")] 
        public bool instantKill;
        public int damage = 1;
        public bool melee;
        public bool byPlayer;
        public bool killAfterAttack;
        public float lifeTime = 5f; // only relevant for bullets right now
        public float cooldown = 2f;
        public GameObject hitAnimation; // only relevant for bullets right now

        [Header("Runtime")] 
        public string originTag;
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