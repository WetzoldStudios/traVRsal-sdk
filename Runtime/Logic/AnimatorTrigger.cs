using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Animator Trigger")]
    public class AnimatorTrigger : MonoBehaviour, IVariableReactor
    {
        [Tooltip("Animator to use. If empty will use first animator found in hierarchy.")]
        public Animator animator;

        public string parameterName;
        public bool invert;
        [Range(0, 5)] public int variableChannel;

        private void Start()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (animator == null) EDebug.LogWarning($"Animator trigger on {gameObject} could not find any attached animator component.");
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (animator == null) return;

            animator.SetBool(parameterName, invert ? !condition : condition);
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }
    }
}