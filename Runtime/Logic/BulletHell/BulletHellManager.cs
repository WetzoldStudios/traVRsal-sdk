using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    public class BulletHellManager : MonoBehaviour
    {
        [Header("Configuration")] public string rotationFrequency;
        public Vector3 rotationAxis = Vector3.up;
        public Transform objectToRotate;
        public string spawnFrequency;

        [Header("Timings")] public float initialDelayMin;
        public float initialDelayMax = 1f;

        [Header("Static References")] public AudioSource audioSource;

        [Header("Events")] public UnityEvent<string> onSpawn;

        private float _nextRotateAction = float.MaxValue;
        private float _nextSpawnAction = float.MaxValue;
        private PatternParser _rotatePattern;
        private PatternParser _spawnPattern;
        private IProjectileShooter _shooter;
        private float _delayPassedTime;

        private void Start()
        {
            if (objectToRotate == null) objectToRotate = transform;
            _shooter = gameObject.GetComponentInChildren<IProjectileShooter>();

            InitSpawnPattern(spawnFrequency);
            InitRotatePattern(rotationFrequency);

            _delayPassedTime = Time.time + Random.Range(initialDelayMin, initialDelayMax);
        }

        private void Update()
        {
            if (Time.time < _delayPassedTime) return;
            if (Time.time >= _nextRotateAction)
            {
                bool skipRotate = false;
                string action = _rotatePattern.GetNextAction();

                // gather delay to next iteration to determine tween length
                float nextRotateDelay = 1f;
                if (!skipRotate)
                {
                    if (_rotatePattern.Next() != null)
                    {
                        nextRotateDelay = (float) _rotatePattern.GetNextExecution() / 1000f;
                        _nextRotateAction = Time.time + nextRotateDelay;
                    }
                    else
                    {
                        _nextRotateAction = float.MaxValue;
                    }
                }

                switch (action)
                {
                    case "delay":
                        break;

                    case "sync":
                        skipRotate = _spawnPattern.GetCurrentStep() > 0;
                        break;

                    default:
                        int degrees = int.Parse(action);
                        objectToRotate.DOLocalRotate(rotationAxis * degrees, nextRotateDelay, RotateMode.LocalAxisAdd);
                        break;
                }
            }

            if (Time.time >= _nextSpawnAction)
            {
                string action = _spawnPattern.GetNextAction();
                switch (action)
                {
                    case "delay":
                        break;

                    case "audio":
                        if (audioSource != null) audioSource.Play();
                        break;

                    default:
                        _shooter?.Fire();
                        onSpawn?.Invoke(action);
                        break;
                }

                if (_spawnPattern.Next() != null)
                {
                    _nextSpawnAction = Time.time + (float) _spawnPattern.GetNextExecution() / 1000f;
                }
                else
                {
                    _nextSpawnAction = float.MaxValue;
                }
            }
        }

        private void InitSpawnPattern(string pattern)
        {
            _spawnPattern = new PatternParser(pattern, true);
            if (_spawnPattern.GetNextExecution() != null)
            {
                _nextSpawnAction = Time.time + (float) _spawnPattern.GetNextExecution() / 1000f;
            }
        }

        private void InitRotatePattern(string pattern)
        {
            _rotatePattern = new PatternParser(pattern, true);
            if (_rotatePattern.GetNextExecution() != null)
            {
                _nextRotateAction = Time.time + (float) _rotatePattern.GetNextExecution() / 1000f;
            }
        }
    }
}