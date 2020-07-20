using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Damageable : ExecutorConfig
    {
        public DamageableConfig config;

        public GameObject damageEffect;
        public GameObject destructionEffect;
        public Vector3 destructionEffectOffset;

        public List<Behaviour> enabledComponentsOnDestruction;
        public List<Behaviour> disabledComponentsOnDestruction;
        public AudioSource[] breakSounds;

        public Damageable()
        {
            enabledComponentsOnDestruction = new List<Behaviour>();
            disabledComponentsOnDestruction = new List<Behaviour>();
        }
    }
}