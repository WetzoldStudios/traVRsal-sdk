using UnityEngine;

namespace traVRsal.SDK
{
    public class JointHelper : MonoBehaviour
    {
        public bool lockXPosition = true;
        public bool lockYPosition = true;
        public bool lockZPosition = true;

        public bool lockXRotation = true;
        public bool lockYRotation = true;
        public bool lockZRotation = true;

        private Vector3 initialPosition;
        private Vector3 initialRotation;

        private void Start()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles;
        }

        private void lockPosition()
        {
            if (lockXPosition || lockYPosition || lockZPosition)
            {
                Vector3 currentPosition = transform.localPosition;
                transform.localPosition = new Vector3(lockXPosition ? initialPosition.x : currentPosition.x, lockYPosition ? initialPosition.y : currentPosition.y, lockZPosition ? initialPosition.z : currentPosition.z);
            }

            if (lockXRotation || lockYRotation || lockZRotation)
            {
                Vector3 currentRotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(lockXRotation ? initialRotation.x : currentRotation.x, lockYRotation ? initialRotation.y : currentRotation.y, lockZRotation ? initialRotation.z : currentRotation.z);
            }
        }

        private void LateUpdate()
        {
            lockPosition();
        }

        private void FixedUpdate()
        {
            lockPosition();
        }
    }
}