using UnityEngine;

namespace traVRsal.SDK
{
    public class DamageInflictor : MonoBehaviour
    {
        public bool instantKill = false;
        public int damage = 1;
        public bool melee = false;
        public bool byPlayer = false;
        public bool killAfterAttack = false;
        public float lifeTime = 1f; // only relevant for bullets right now
        public GameObject hitAnimation; // only relevant for bullets right now
        public string originTag;

        void Start()
        {
            if (string.IsNullOrEmpty(originTag)) originTag = gameObject.tag;
        }
    }
}