using UnityEngine;

namespace traVRsal.SDK
{
    public class ProjectileShooter : MonoBehaviour
    {
        public GameObject bullet;
        public Transform firePoint;
        public GameObject layerRef;
        public float speed = 1f;
        public bool countIntoStatistics = false;
        public bool byPlayer = false;
        public AudioSource audioSource;
        public DamageInflictor damageInflictor;

        void Start() { }
    }
}