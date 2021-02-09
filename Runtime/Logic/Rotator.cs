using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour
    {
        public Vector2 degreesPerSecond = new Vector2(10f, 10f);
        public Vector3 axis = Vector3.up;

        private float finalDegreesPerSecond;

        private void Start()
        {
            finalDegreesPerSecond = Random.Range(degreesPerSecond.x, degreesPerSecond.y);
        }

        private void Update()
        {
            // TODO: switch to DOTween to get rid of update loop
            transform.Rotate(axis, finalDegreesPerSecond * Time.deltaTime);
        }
    }
}