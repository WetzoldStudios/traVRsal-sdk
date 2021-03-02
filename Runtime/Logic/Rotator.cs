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
        public Vector2 degrees = new Vector2(10f, 10f);
        public float duration = 2f;
        public Vector3 axis = Vector3.up;

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
            startTime = Time.time + onDelay;
        }

        private void Update()
        {
            if (mode != Mode.Manual) return;
            if (Time.time < startTime) return;

            // TODO: switch to DOTween to get rid of update loop
            transform.Rotate(axis, finalDegrees * Time.deltaTime);
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                transform.DOLocalRotate(originalRotation + axis * finalDegrees, duration).SetDelay(onDelay + (changedOnce ? 0f : onDelay));
            }
            else
            {
                transform.DOLocalRotateQuaternion(originalRotationQ, duration).SetDelay(offDelay + (changedOnce ? 0f : onDelay));
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }
    }
}