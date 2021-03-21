using UnityEngine;

namespace traVRsal.SDK
{
    public class PlayerDetector : ExecutorConfig
    {
        public bool limitDetectionRadius = true;

        [Tooltip("Cone of vision in case limit detection is activated")]
        public float maxAngle = 70f;

        [Tooltip("Transform which will be used as the anchor for forward vision")]
        public Transform scanDirection;

        [Tooltip("Defines if actions based on visibility should be triggered, e.g. speech if defined. Deactivate for slight performance improvements.")]
        public bool reportVisibility = true;
    }
}