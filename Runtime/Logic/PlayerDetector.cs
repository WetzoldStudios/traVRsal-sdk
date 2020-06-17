using UnityEngine;

namespace traVRsal.SDK
{
    public class PlayerDetector : MonoBehaviour
    {
        public bool limitDetectionRadius = true;
        public float maxAngle = 70f;
        public Transform scanDirection;
        public bool reportVisibility = true;

        void Start() { }
    }
}