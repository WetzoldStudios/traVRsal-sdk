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

        private ISoundAction context;
        private bool initDone;
        private bool triggered;

        private void Start()
        {
            initDone = true;
            ISoundAction[] contexts = GetComponentsInParent<ISoundAction>(true);
            if (contexts.Length > 0)
            {
                context = contexts[0];
            }
            else
            {
                EDebug.LogError($"Could not find context on {gameObject}");
            }
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (stateToReactTo != VariableState.Any)
            {
                if (condition && stateToReactTo != VariableState.True) return;
                if (!condition && stateToReactTo != VariableState.False) return;
            }

            if (!playOnlyOnce || !triggered)
            {
                if (audio != null) audio.Play();
                if (!string.IsNullOrWhiteSpace(music) && context != null) context.PlayMusic(music);
            }
            triggered = true;
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }
    }
}