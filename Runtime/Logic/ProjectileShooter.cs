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

        public ReloadType reloadType = ReloadType.Never;
        public int bulletCount;
        public GameObject bullet;
        public Transform firePoint;
        public GameObject layerRef;
        public float speed = 1f;
        public bool countIntoStatistics;
        public bool byPlayer;
        public AudioSource audioSource;
        public AudioSource reloadAudio;
        public DamageInflictor damageInflictor;

        private IProjectileShooter shooter;

        private void Start()
        {
            shooter = GetComponent<IProjectileShooter>();
        }

        public void Fire()
        {
            shooter?.Fire();
        }

        public void Reload()
        {
            shooter?.Reload();
        }
    }
}