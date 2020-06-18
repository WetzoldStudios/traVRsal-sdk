using UnityEngine;

namespace traVRsal.SDK
{
    public class PlayerDetector : ExecutorConfig
    {
        public bool limitDetectionRadius = true;
        public float maxAngle = 70f;
        public Transform scanDirection;
        public bool reportVisibility = true;
    }
}