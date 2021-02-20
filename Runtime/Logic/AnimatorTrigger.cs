using UnityEngine;

namespace traVRsal.SDK
{
    public class AnimatorTrigger : MonoBehaviour, IVariableReactor
    {
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