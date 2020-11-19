using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour
    {
        public float degreesPerSecond = 10f;
        public Vector3 axis = Vector3.up;

        private void Update()
        {
            transform.Rotate(axis, degreesPerSecond * Time.deltaTime);
        }
    }
}