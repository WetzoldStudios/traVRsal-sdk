using UnityEngine;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Audio Player")]
    public class AudioPlayer : MonoBehaviour, IWorldStateReactor, IVariableReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        public enum VariableState
        {
            Any = 0,
            True = 1,
            False = 2
        }

        [Header("Configuration")] public Mode mode = Mode.Variable;
        [Range(0, 5)] public int variableChannel;

        public VariableState stateToReactTo = VariableState.Any;

        [Tooltip("Sound to play when the variable is true.")]
        public AudioSource audio;

        public AudioClip[] randomClips;

        [Tooltip("Music to play when the variable is true, referring to a file under Audio/Music.")]
        public string music;

        public bool playOnlyOnce;

        [Header("Timings")] public Vector2 initialDelay;

        [Tooltip("Random pause after playing an audio clip.")]
        public Vector2 delay = new Vector2(4f, 8f);

        private float _finalInitialDelay;

        private ISoundAction _context;
        private bool _loadingDone;
        private bool _initStateDone;
        private bool _initDone;
        private bool _triggered;
        private float _nextTime;

        private void Start()
        {
            if (!_initStateDone) InitState();

            Init();
        }

        private void InitState()
        {
            _finalInitialDelay = Random.Range(initialDelay.x, initialDelay.y);

            _initStateDone = true;
        }

        private void Update()
        {
            if (!_initDone || mode != Mode.Manual || audio == null) return;

            if (_nextTime > 0 && Time.time > _nextTime && (!playOnlyOnce || !_triggered))
            {
                PlayAudio();
                _nextTime = Time.time + audio.clip.length + Random.Range(delay.x, delay.y);
            }
        }

        private void OnEnable()
        {
            if (!_loadingDone) return;
            if (_nextTime > 0) return;

            // needed for support of initialDelay, since any WaitForSeconds will be interrupted during loading when GO becomes inactive
            _nextTime = Mathf.Max(Time.time + _finalInitialDelay, float.Epsilon);
        }

        private void Init()
        {
            _initDone = true;

            if (!string.IsNullOrWhiteSpace(music))
            {
                ISoundAction[] contexts = GetComponentsInParent<ISoundAction>(true);
                if (contexts.Length > 0)
                {
                    _context = contexts[0];
                }
                else
                {
                    EDebug.LogError($"Could not find context on {gameObject}");
                }
            }
        }

        private void PlayAudio()
        {
            _triggered = true;
            if (audio == null || !audio.enabled || !audio.gameObject.activeSelf) return;

            if (randomClips != null && randomClips.Length > 0) audio.clip = randomClips[Random.Range(0, randomClips.Length)];
            audio.Play();
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (!_initDone) Init();
            if (stateToReactTo != VariableState.Any)
            {
                if (condition && stateToReactTo != VariableState.True) return;
                if (!condition && stateToReactTo != VariableState.False) return;
            }

            if (!playOnlyOnce || !_triggered)
            {
                PlayAudio();
                if (!string.IsNullOrWhiteSpace(music) && _context != null) _context.PlayMusic(music);
            }
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            if (!_initStateDone) InitState();

            _loadingDone = true;
            if (instantEnablement) OnEnable();
        }
    }
}