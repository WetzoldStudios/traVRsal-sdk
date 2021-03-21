using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour, IVariableReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        public Mode mode = Mode.Manual;
        [Tooltip("Degrees per second in manual mode. Total degrees otherwise. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 degrees = new Vector2(10f, 10f);
        [Tooltip("Local axis around which the object should be rotated, e.g. (0,1,0) for Y.")]
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;

        public float duration = 2f;
        public float onDelay;
        public float offDelay;

        private float finalDegrees;
        private Quaternion originalRotationQ;
        private Vector3 originalRotation;
        private float startTime;
        private bool changedOnce;

        private void Start()
        {
            originalRotationQ = transform.localRotation;
            originalRotation = originalRotationQ.eulerAngles;
            finalDegrees = Random.Range(degrees.x, degrees.y);

            if (mode == Mode.Manual)
            {
                startTime = Time.time + onDelay;
                if (!loop)
                {
                    transform.DOLocalRotate(originalRotation + axis * finalDegrees, duration).SetDelay(onDelay).SetEase(easeType);
                }
            }
        }

        private void Update()
        {
            if (mode != Mode.Manual || !loop) return;
            if (Time.time < startTime) return;

            // TODO: switch to DOTween to get rid of update loop
            transform.Rotate(axis, finalDegrees * Time.deltaTime);
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                transform.DOLocalRotate(originalRotation + axis * finalDegrees, duration).SetDelay(onDelay + (changedOnce ? 0f : onDelay)).SetEase(easeType);
            }
            else
            {
                transform.DOLocalRotateQuaternion(originalRotationQ, duration).SetDelay(offDelay + (changedOnce ? 0f : onDelay)).SetEase(easeType);
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }
    }
}