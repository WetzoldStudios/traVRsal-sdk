using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    public class Wave : MonoBehaviour
    {
        public string pattern;
        public BasicSpawnRule[] spawnRules;

        [Space] public UnityEvent onOver;
        public UnityEvent onDefended;

        private float _nextSpawnAction = float.MaxValue;
        private PatternParser _spawnPattern;
        private ISpawner _spawner;

        private void Start()
        {
            _spawner = GetComponentInParent<ISpawner>();
            InitSpawnPattern(pattern);
        }

        private void Update()
        {
            if (Time.time >= _nextSpawnAction)
            {
                string action = _spawnPattern.GetNextAction();

                // find spawn rule
                BasicSpawnRule rule = spawnRules.FirstOrDefault(sr => sr.key == action);
                if (rule != null)
                {
                    StartCoroutine(_spawner.Spawn(rule));
                }
                else
                {
                    EDebug.LogError($"Spawn rule {action} is not defined in wave {gameObject.name}");
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

        private void InitSpawnPattern(string spawnPattern)
        {
            _spawnPattern = new PatternParser(spawnPattern, true);
            if (_spawnPattern.GetNextExecution() != null)
            {
                _nextSpawnAction = Time.time + (float) _spawnPattern.GetNextExecution() / 1000f;
            }
        }

        public override string ToString()
        {
            return $"Wave ({pattern})";
        }
    }
}