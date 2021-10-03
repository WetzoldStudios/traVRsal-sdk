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

        private Vector3 _initialPosition;
        private Vector3 _initialRotation;

        private void Start()
        {
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localEulerAngles;
        }

        private void LockPosition()
        {
            if (lockXPosition || lockYPosition || lockZPosition)
            {
                Vector3 currentPosition = transform.localPosition;
                transform.localPosition = new Vector3(lockXPosition ? _initialPosition.x : currentPosition.x, lockYPosition ? _initialPosition.y : currentPosition.y, lockZPosition ? _initialPosition.z : currentPosition.z);
            }

            if (lockXRotation || lockYRotation || lockZRotation)
            {
                Vector3 currentRotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(lockXRotation ? _initialRotation.x : currentRotation.x, lockYRotation ? _initialRotation.y : currentRotation.y, lockZRotation ? _initialRotation.z : currentRotation.z);
            }
        }

        private void LateUpdate()
        {
            LockPosition();
        }

        private void FixedUpdate()
        {
            LockPosition();
        }
    }
}