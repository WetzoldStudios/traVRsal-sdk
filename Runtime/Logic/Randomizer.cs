using UnityEngine;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    public class Randomizer : MonoBehaviour
    {
        [Header("Move")] public bool randomMovement;

        [Tooltip("Distance the object should be moved. Use different values for X and Y to define a range for a random distance.")]
        public Vector2 moveX = new Vector2(0f, 0f);

        [Tooltip("Distance the object should be moved. Use different values for X and Y to define a range for a random distance.")]
        public Vector2 moveY = new Vector2(0f, 0f);

        [Tooltip("Distance the object should be moved. Use different values for X and Y to define a range for a random distance.")]
        public Vector2 moveZ = new Vector2(0f, 0f);

        [Header("Rotate")] public bool randomRotation;

        [Tooltip("Degrees to rotate. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 rotateX = new Vector2(0f, 0f);

        [Tooltip("Degrees to rotate. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 rotateY = new Vector2(0f, 0f);

        [Tooltip("Degrees to rotate. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 rotateZ = new Vector2(0f, 0f);

        [Header("Scale")] public bool randomScale;

        [Tooltip("Scale of the object, e.g. (1.2,1.2) for 20% bigger. Use different values for X and Y to define a range for a random scale.")]
        public Vector2 scaleX = new Vector2(0f, 0f);

        [Tooltip("Scale of the object, e.g. (1.2,1.2) for 20% bigger. Use different values for X and Y to define a range for a random scale.")]
        public Vector2 scaleY = new Vector2(0f, 0f);

        [Tooltip("Scale of the object, e.g. (1.2,1.2) for 20% bigger. Use different values for X and Y to define a range for a random scale.")]
        public Vector2 scaleZ = new Vector2(0f, 0f);

        private void Start()
        {
            // apply to all children
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (randomMovement)
                {
                    child.localPosition += new Vector3(Random.Range(moveX.x, moveX.y), Random.Range(moveY.x, moveY.y), Random.Range(moveZ.x, moveZ.y));
                }
                if (randomRotation)
                {
                    child.localEulerAngles += new Vector3(Random.Range(rotateX.x, rotateX.y), Random.Range(rotateY.x, rotateY.y), Random.Range(rotateZ.x, rotateZ.y));
                }
                if (randomScale)
                {
                    child.localScale += new Vector3(Random.Range(scaleX.x, scaleX.y), Random.Range(scaleY.x, scaleY.y), Random.Range(scaleZ.x, scaleZ.y));
                }
            }
        }
    }
}