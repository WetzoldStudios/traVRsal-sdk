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
        private bool initDone;
        
        private void Init()
        {
            initDone = true;
            IVariableAction[] contexts = GetComponentsInParent<IVariableAction>(true);
            if (contexts.Length > 0)
            {
                context = contexts[0];
            }
            else
            {
                EDebug.LogError($"Could not find context on {gameObject}");
            }
        }

        public void ReachActionMin()
        {
            if (!initDone) Init();
            if (minSound != null && minSound.clip != null) minSound.Play();

            context.ReachActionMin(variableChannel);
        }

        public void ReachActionMax()
        {
            if (!initDone) Init();
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            context.ReachActionMax(variableChannel);
        }

        public void ToggleAction()
        {
            if (!initDone) Init();
            context.ToggleAction(variableChannel);
        }
    }
}