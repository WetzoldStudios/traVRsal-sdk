using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    public class Wave : MonoBehaviour
    {
        public string pattern;
        public bool loop;
        public SpawnRule[] spawnRules = new SpawnRule[1]; // init with 1 elements to have inspector filled with correct default values

        [Header("Events")] public UnityEvent onStarted;
        public UnityEvent onOver;
        public UnityEvent onDefended;

        private bool _isOver;
        private bool _isDefended;
        private float _nextSpawnAction = float.MaxValue;
        private PatternParser _spawnPattern;
        private ISpawner _spawner;
        private List<GameObject> _spawnedGos;

        private void Start()
        {
            _spawnedGos = new List<GameObject>();
            _spawner = GetComponentInParent<ISpawner>();
            Restart();
        }

        private void Update()
        {
            if (Time.time >= _nextSpawnAction)
            {
                string action = _spawnPattern.GetNextAction();

                // find spawn rule
                SpawnRule rule = spawnRules.FirstOrDefault(sr => sr.key == action);
                if (rule != null)
                {
                    StartCoroutine(Spawn(rule));
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
                    _isOver = true;
                    onOver?.Invoke();
                }
            }

            if (_isOver && !_isDefended)
            {
                _isDefended = _spawnedGos.All(go => go == null);
                if (_isDefended) onDefended?.Invoke();
            }
        }

        private IEnumerator Spawn(SpawnRule rule)
        {
            yield return _spawner.Spawn(rule, go => _spawnedGos.Add(go));
        }

        public void Restart()
        {
            _isDefended = false;
            _isOver = false;

            InitSpawnPattern(pattern);
            onStarted?.Invoke();
        }

        private void InitSpawnPattern(string spawnPattern)
        {
            _spawnPattern = new PatternParser(spawnPattern, loop);
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