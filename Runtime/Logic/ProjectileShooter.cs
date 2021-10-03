using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Projectile Shooter")]
    public class ProjectileShooter : ExecutorConfig
    {
        public enum ReloadType
        {
            Never = 0,
            PointDown = 1
        }

        [Header("Configuration")] public ReloadType reloadType = ReloadType.Never;
        public int bulletCount;
        public float speed = 1f;
        public bool countIntoStatistics;
        public bool byPlayer;

        [Header("Static References")] public GameObject bullet;
        public Transform firePoint;
        public GameObject layerRef;
        public DamageInflictor damageInflictor;

        [Header("Audio")] public AudioSource fireAudio;
        public AudioSource reloadAudio;

        [Header("Animation")] public Animation animation;
        public string fireAnimation;
        public float fireAnimationSpeed = 1f;
        public string reloadAnimation;
        public float reloadAnimationSpeed = 1f;

        private IProjectileShooter _shooter;

        private void Start()
        {
            _shooter = GetComponent<IProjectileShooter>();
        }

        public void Fire()
        {
            _shooter?.Fire();
        }

        public void Reload()
        {
            _shooter?.Reload();
        }
    }
}