using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Mover : MonoBehaviour, IVariableReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;

        [Tooltip("Distance the object should travel. Use different values for X and Y to define a range for a random distance.")]
        public Vector2 distance = new Vector2(1f, 1f);

        [Tooltip("Direction the object should travel, e.g. (0,1,0) for up.")]
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        [Header("Timing")] public float initialDelay;
        public float duration = 4f;
        public float onDelay;
        public float offDelay;

        private float finalDistance;
        private Vector3 originalPosition;
        private bool changedOnce;

        private void Start()
        {
            originalPosition = transform.localPosition;
            finalDistance = Random.Range(distance.x, distance.y);

            if (mode == Mode.Manual)
            {
                transform.DOLocalMove(transform.localPosition + axis * finalDistance, duration).SetLoops(loop ? -1 : 0, loopType).SetDelay(initialDelay).SetEase(easeType);
            }
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                transform.DOLocalMove(originalPosition + axis * finalDistance, duration).SetDelay(onDelay + (changedOnce ? 0f : initialDelay)).SetEase(easeType);
            }
            else
            {
                transform.DOLocalMove(originalPosition, duration).SetDelay(offDelay + (changedOnce ? 0f : initialDelay)).SetEase(easeType);
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }
    }
}