using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Variable Action")]
    public class VariableAction : MonoBehaviour
    {
        [Range(0, 5)] public int variableChannel;

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

            context.ReachActionMin(variableChannel);
        }

        public void ReachActionMax()
        {
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            context.ReachActionMax(variableChannel);
        }

        public void ToggleAction()
        {
            context.ToggleAction(variableChannel);
        }
    }
}