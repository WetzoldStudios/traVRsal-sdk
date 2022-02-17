using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Damageable")]
    public class Damageable : ExecutorConfig, IDataSource
    {
        public DamageableConfig config;

        public GameObject damageEffect;
        public GameObject destructionEffect;
        public Vector3 destructionEffectOffset;

        public UnityEvent onDestruction;
        public AudioSource[] breakSounds;

        public Damageable()
        {
        }
    }
}