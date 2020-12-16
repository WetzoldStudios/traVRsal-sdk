using UnityEngine;

namespace traVRsal.SDK
{
    public class AudioPlayer : MonoBehaviour, IVariableReactor
    {
        public AudioSource audioActive;
        public bool playActiveOnlyOnce;

        public AudioSource audioInactive;
        public bool playInactiveOnlyOnce;

        private bool activeTriggered;
        private bool inactiveTriggered;

        public void VariableChanged(Variable variable, bool condition)
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