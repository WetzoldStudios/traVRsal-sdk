using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour
    {
        public Vector2 degreesPerSecond = new Vector2(10f, 10f);
        public Vector3 axis = Vector3.up;
        public float initialDelay;

        private float finalDegreesPerSecond;
        private float startTime;

        private void Start()
        {
            finalDegreesPerSecond = Random.Range(degreesPerSecond.x, degreesPerSecond.y);
            startTime = Time.time + initialDelay;
        }

        private void Update()
        {
            if (Time.time < startTime) return;

            // TODO: switch to DOTween to get rid of update loop
            transform.Rotate(axis, finalDegreesPerSecond * Time.deltaTime);
        }
    }
}