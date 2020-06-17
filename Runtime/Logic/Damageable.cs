using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Damageable : MonoBehaviour
    {
        public DamageableConfig config;

        public GameObject damageEffect;
        public GameObject destructionEffect;
        public Vector3 destructionEffectOffset;

        public List<Behaviour> enabledComponentsOnDestruction;
        public List<Behaviour> disabledComponentsOnDestruction;
        public AudioSource[] breakSounds;

        void Start() { }
    }
}