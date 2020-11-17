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
        public int bulletCount = 0;
        public GameObject bullet;
        public Transform firePoint;
        public GameObject layerRef;
        public float speed = 1f;
        public bool countIntoStatistics = false;
        public bool byPlayer;
        public AudioSource audioSource;
        public DamageInflictor damageInflictor;

        private IProjectileShooter shooter;

        public void Fire()
        {
            if (shooter == null) shooter = GetComponent<IProjectileShooter>();

            shooter?.Fire();
        }
    }
}