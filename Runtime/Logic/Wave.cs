using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    public class Wave : MonoBehaviour, IWorldStateReactor
    {
        public enum Mode
        {
            Automatic = 0,
            Manual = 1
        }

        public Mode mode = Mode.Automatic;
        public string pattern;
        public bool loop;
        public SpawnRule[] spawnRules = new SpawnRule[1]; // init with 1 elements to have inspector filled with correct default values

        [Header("Events")] public UnityEvent onStarted;
        public UnityEvent onOver;
        public UnityEvent onDefended;

        private bool _initDone;
        private bool _isOver;
        private bool _isDefended;
        private bool _inProgress;
        private float _nextSpawnAction = float.MaxValue;
        private PatternParser _spawnPattern;
        private ISpawner _spawner;
        private List<GameObject> _spawnedGos;

        private void Awake()
        {
            _spawnedGos = new List<GameObject>();
            _nextSpawnAction = float.MaxValue;
        }

        private void Start()
        {
            _spawner = GetComponentInParent<ISpawner>();
        }

        private void OnEnable()
        {
            if (_initDone && mode == Mode.Automatic) Trigger();
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
                    _inProgress = true;
                    StartCoroutine(Spawn(rule));
                }
                else
                {
                    EDebug.LogError($"Spawn rule '{action}' is not defined in wave '{gameObject.name}'");
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

            if (_isOver && !_isDefended && !_inProgress)
            {
                _isDefended = _spawnedGos.All(go => go == null);
                if (_isDefended) TriggerDefended();
            }
        }

        private IEnumerator Spawn(SpawnRule rule)
        {
            yield return _spawner.Spawn(rule, go => _spawnedGos.Add(go));
            _inProgress = false;
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            if (_nextSpawnAction < float.MaxValue) return;

            _isDefended = false;
            _isOver = false;

            InitSpawnPattern(pattern);
            onStarted?.Invoke();
        }

        [ContextMenu("Retrigger")]
        public void Retrigger()
        {
            _nextSpawnAction = float.MaxValue;
            Trigger();
        }

        [ContextMenu("Set To Defended")]
        public void TriggerDefended()
        {
            onDefended?.Invoke();
        }

        private void InitSpawnPattern(string spawnPattern)
        {
            if (string.IsNullOrWhiteSpace(spawnPattern) && spawnRules.Length > 0)
            {
                // auto-generate a pattern using all existing spawn rules in case there is none
                spawnPattern = string.Join(",", spawnRules.Select(sr => "0," + sr.key));
            }

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

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            _initDone = true;
            if (mode == Mode.Automatic) Trigger();
        }
    }
}