using UnityEngine;

namespace traVRsal.SDK
{
    public class ProjectileShooter : ExecutorConfig
    {
        public GameObject bullet;
        public Transform firePoint;
        public GameObject layerRef;
        public float speed = 1f;
        public bool countIntoStatistics = false;
        public bool byPlayer = false;
        public AudioSource audioSource;
        public DamageInflictor damageInflictor;

        private IProjectileShooter shooter;

        public void Fire()
        {
            if (shooter == null) shooter = GetComponent<IProjectileShooter>();
            if (shooter == null) return;

            shooter.Fire();
        }
    }
}