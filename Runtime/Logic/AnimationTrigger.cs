using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class AnimationTrigger : MonoBehaviour, IVariableReactor
    {
        public Vector3 direction = Vector3.right;
        public float duration = 3f;

        private Vector3 originalPosition;

        void Start()
        {
            originalPosition = transform.localPosition;
        }

        public void VariableChanged(Variable variable, bool condition)
        {
            if (condition)
            {
                transform.DOLocalMove(originalPosition + direction, duration);
            }
            else
            {
                transform.DOLocalMove(originalPosition, duration);
            }
        }

    }
}