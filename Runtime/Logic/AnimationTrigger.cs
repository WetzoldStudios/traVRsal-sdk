using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class AnimationTrigger : MonoBehaviour, IVariableReactor
    {
        public Vector3 direction = Vector3.right;
        public float duration = 3f;
        public float initialDelay;
        public float onDelay;
        public float offDelay;

        private Vector3 originalPosition;
        private bool changedOnce;

        private void Start()
        {
            originalPosition = transform.localPosition;
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (condition)
            {
                transform.DOLocalMove(originalPosition + direction, duration).SetDelay(onDelay + (changedOnce ? 0f : initialDelay));
            }
            else
            {
                transform.DOLocalMove(originalPosition, duration).SetDelay(offDelay + (changedOnce ? 0f : initialDelay));
            }

            if (!initialCall) changedOnce = true;
        }
    }
}