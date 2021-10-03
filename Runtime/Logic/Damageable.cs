using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Damageable")]
    public class Damageable : ExecutorConfig
    {
        public DamageableConfig config;

        public GameObject damageEffect;
        public GameObject destructionEffect;
        public Vector3 destructionEffectOffset;

        [Obsolete] public List<GameObject> enabledGameObjectsOnDestruction;
        [Obsolete] public List<GameObject> disabledGameObjectsOnDestruction;
        [Obsolete] public List<Behaviour> enabledComponentsOnDestruction;
        [Obsolete] public List<Behaviour> disabledComponentsOnDestruction;
        public UnityEvent onDestruction;
        public AudioSource[] breakSounds;

        public Damageable()
        {
            enabledGameObjectsOnDestruction = new List<GameObject>();
            disabledGameObjectsOnDestruction = new List<GameObject>();
            enabledComponentsOnDestruction = new List<Behaviour>();
            disabledComponentsOnDestruction = new List<Behaviour>();
        }
    }
}