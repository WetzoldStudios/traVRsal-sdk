using UnityEngine;

namespace traVRsal.SDK
{
    public class DamageInflictor : MonoBehaviour
    {
        public bool instantKill;
        public int damage = 1;
        public bool melee;
        public bool byPlayer;
        public bool killAfterAttack;
        public float lifeTime = 5f; // only relevant for bullets right now
        public GameObject hitAnimation; // only relevant for bullets right now
        public string originTag;

        void Start()
        {
            if (string.IsNullOrEmpty(originTag)) originTag = gameObject.tag;
        }
    }
}