using UnityEngine;

namespace traVRsal.SDK
{
    public class AudioPlayer : MonoBehaviour, IVariableReactor
    {
        [Tooltip("Sound to play when the variable is true.")]
        public AudioSource audioActive;
        public bool playActiveOnlyOnce;

        [Tooltip("Sound to play when the variable is false.")]
        public AudioSource audioInactive;
        public bool playInactiveOnlyOnce;

        private bool activeTriggered;
        private bool inactiveTriggered;

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (condition)
            {
                if (!playActiveOnlyOnce || !activeTriggered)
                {
                    if (audioActive != null) audioActive.Play();
                }
                activeTriggered = true;
            }
            else
            {
                if (!playInactiveOnlyOnce || !inactiveTriggered)
                {
                    if (audioInactive != null) audioInactive.Play();
                }
                inactiveTriggered = true;
            }
        }
    }
}