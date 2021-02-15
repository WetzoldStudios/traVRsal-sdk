using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    public class Button : MonoBehaviour
    {
        public float minLocalZ = 0.25f;
        public float maxLocalZ = 0.55f;
        public float tolerance = 0.01f;

        public UnityEvent OnButtonDown;
        public UnityEvent OnButtonUp;

        private bool pushingDown;

        private void Start()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, maxLocalZ);
        }

        private void Update()
        {
            Vector3 buttonDownPosition = GetButtonDownPosition();
            Vector3 buttonUpPosition = GetButtonUpPosition();

            if (transform.localPosition.z < minLocalZ)
            {
                transform.localPosition = buttonDownPosition;
            }
            else if (transform.localPosition.z > maxLocalZ)
            {
                transform.localPosition = buttonUpPosition;
            }

            float buttonDownDistance = transform.localPosition.z - buttonDownPosition.z;
            if (buttonDownDistance <= tolerance && !pushingDown)
            {
                pushingDown = true;
                OnButtonDown?.Invoke();
            }
            float buttonUpDistance = buttonUpPosition.z - transform.localPosition.z;
            if (buttonUpDistance <= tolerance && pushingDown)
            {
                pushingDown = false;
                OnButtonUp?.Invoke();
            }
        }

        private Vector3 GetButtonUpPosition()
        {
            return new Vector3(transform.localPosition.x, transform.localPosition.y, maxLocalZ);
        }

        private Vector3 GetButtonDownPosition()
        {
            return new Vector3(transform.localPosition.x, transform.localPosition.y, minLocalZ);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;

            Vector3 upPosition = transform.TransformPoint(new Vector3(transform.localPosition.x, transform.localPosition.y, maxLocalZ));
            Vector3 downPosition = transform.TransformPoint(new Vector3(transform.localPosition.x, transform.localPosition.y, minLocalZ));

            Vector3 size = new Vector3(0.005f, 0.005f, 0.005f);

            Gizmos.DrawCube(upPosition, size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(downPosition, size);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(upPosition, downPosition);
        }
    }
}