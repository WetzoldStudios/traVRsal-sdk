using UnityEngine;

namespace traVRsal.SDK
{
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

        public void Fire()
        {
            if (shooter == null) shooter = GetComponent<IProjectileShooter>();

            shooter?.Fire();
        }
    }
}