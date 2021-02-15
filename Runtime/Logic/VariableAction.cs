using UnityEngine;

namespace traVRsal.SDK
{
    public class VariableAction : MonoBehaviour
    {
        public AudioSource maxSound;
        public AudioSource minSound;

        private IVariableAction context;

        private void Start()
        {
            context = GetComponentInParent<IVariableAction>();
        }

        public void ReachActionMin()
        {
            if (minSound != null && minSound.clip != null) minSound.Play();

            context.ReachActionMin();
        }

        public void ReachActionMax()
        {
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            context.ReachActionMax();
        }

        public void ToggleAction()
        {
            context.ToggleAction();
        }
    }
}