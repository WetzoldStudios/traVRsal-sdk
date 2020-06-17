using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class BulletHellManager : MonoBehaviour
    {
        public string rotationFrequency;
        public Vector3 rotationAxis = Vector3.up;
        public Transform objectToRotate;
        public AudioSource audioSource;
        public string spawnFrequency;
        public float initialDelayMin = 0f;
        public float initialDelayMax = 1f;

        private float nextRotateAction = float.MaxValue;
        private float nextSpawnAction = float.MaxValue;
        private PatternParser rotatePattern;
        private PatternParser spawnPattern;
        private IProjectileShooter shooter;
        private float delayPassedTime;

        void Start()
        {
            if (objectToRotate == null) objectToRotate = transform;
            shooter = gameObject.GetComponentInChildren<IProjectileShooter>();

            InitSpawnPattern(spawnFrequency);
            InitRotatePattern(rotationFrequency);

            delayPassedTime = Time.time + Random.Range(initialDelayMin, initialDelayMax);
        }

        void Update()
        {
            if (Time.time < delayPassedTime) return;
            if (Time.time >= nextRotateAction)
            {
                bool skipRotate = false;
                string action = rotatePattern.GetNextAction();

                // gather delay to next iteration to determine tween length
                float nextRotateDelay = 1f;
                if (!skipRotate)
                {
                    if (rotatePattern.Next() != null)
                    {
                        nextRotateDelay = (float)rotatePattern.GetNextExecution() / 1000f;
                        nextRotateAction = Time.time + nextRotateDelay;
                    }
                    else
                    {
                        nextRotateAction = float.MaxValue;
                    }
                }

                switch (action)
                {
                    case "delay":
                        break;

                    case "sync":
                        skipRotate = spawnPattern.GetCurrentStep() > 0;
                        break;

                    default:
                        int degrees = int.Parse(action);
                        objectToRotate.DOLocalRotate(rotationAxis * degrees, nextRotateDelay, RotateMode.LocalAxisAdd);
                        break;
                }
            }

            if (Time.time >= nextSpawnAction)
            {
                string action = spawnPattern.GetNextAction();
                switch (action)
                {
                    case "delay":
                        break;

                    case "audio":
                        if (audioSource != null) audioSource.Play();
                        break;

                    default:
                        shooter.Fire();
                        break;

                }

                if (spawnPattern.Next() != null)
                {
                    nextSpawnAction = Time.time + (float)spawnPattern.GetNextExecution() / 1000f;
                }
                else
                {
                    nextSpawnAction = float.MaxValue;
                }
            }
        }

        private void InitSpawnPattern(string pattern)
        {
            spawnPattern = new PatternParser(pattern, true);
            if (spawnPattern.GetNextExecution() != null)
            {
                nextSpawnAction = Time.time + (float)spawnPattern.GetNextExecution() / 1000f;
            }
        }

        private void InitRotatePattern(string pattern)
        {
            rotatePattern = new PatternParser(pattern, true);
            if (rotatePattern.GetNextExecution() != null)
            {
                nextRotateAction = Time.time + (float)rotatePattern.GetNextExecution() / 1000f;
            }
        }
    }
}