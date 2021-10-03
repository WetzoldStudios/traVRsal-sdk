using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Return To Position")]
    public class ReturnToPosition : MonoBehaviour
    {
        public float speed = 10f;
        public bool resetLayer;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private int _originalLayer;
        private bool _doReturn;

        private void Start()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
            _originalLayer = gameObject.layer;
        }

        private void Update()
        {
            if (!_doReturn) return;

            transform.position = Vector3.Lerp(transform.position, _originalPosition, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, speed * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, _originalPosition);

            // reached destination, snap to final transform position
            if (dist <= 0.002f)
            {
                _doReturn = false;
                transform.position = _originalPosition;
                transform.rotation = _originalRotation;
            }
        }

        public void Trigger()
        {
            _doReturn = true;
            if (resetLayer) gameObject.layer = _originalLayer;
        }

        public void Stop()
        {
            _doReturn = false;
        }
    }
}