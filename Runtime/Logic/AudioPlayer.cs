using UnityEngine;

namespace traVRsal.SDK
{
    public class AudioPlayer : MonoBehaviour, IVariableReactor
    {
        public AudioSource audioActive;
        public AudioSource audioInactive;

        public void VariableChanged(Variable variable, bool condition)
        {
            if (condition)
            {
                if (audioActive != null) audioActive.Play();
            }
            else
            {
                if (audioInactive != null) audioInactive.Play();
            }
        }

    }
}