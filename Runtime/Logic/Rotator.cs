using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour
    {
        public float speed = 20f;
        public float degrees = 1f;

        void Update()
        {
            transform.Rotate(0, degrees * Time.deltaTime * speed, 0);
        }
    }
}