using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Damageable")]
    public class Damageable : ExecutorConfig
    {
        public DamageableConfig config;

        public GameObject damageEffect;
        public GameObject destructionEffect;
        public Vector3 destructionEffectOffset;

        public List<GameObject> enabledGameObjectsOnDestruction;
        public List<GameObject> disabledGameObjectsOnDestruction;
        public List<Behaviour> enabledComponentsOnDestruction;
        public List<Behaviour> disabledComponentsOnDestruction;
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