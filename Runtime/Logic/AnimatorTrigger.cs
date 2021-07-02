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

        private void Start()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            animator.SetBool(parameterName, invert ? !condition : condition);
        }
    }
}