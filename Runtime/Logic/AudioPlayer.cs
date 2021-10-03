using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Audio Player")]
    public class AudioPlayer : MonoBehaviour, IVariableReactor
    {
        public enum VariableState
        {
            Any = 0,
            True = 1,
            False = 2
        }

        [Range(0, 5)] public int variableChannel;

        public VariableState stateToReactTo = VariableState.Any;

        [Tooltip("Sound to play when the variable is true.")]
        public AudioSource audio;

        [Tooltip("Music to play when the variable is true, referring to a file under Audio/Music.")]
        public string music;

        public bool playOnlyOnce;

        private ISoundAction _context;
        private bool _initDone;
        private bool _triggered;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _initDone = true;

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
                if (audio != null) audio.Play();
                if (!string.IsNullOrWhiteSpace(music) && _context != null) _context.PlayMusic(music);
            }
            _triggered = true;
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }
    }
}