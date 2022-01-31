using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Player Detector")]
    public class PlayerDetector : ExecutorConfig
    {
        public bool limitDetectionRadius = true;

        [Tooltip("Cone of vision in case radius limit detection is activated")]
        public float maxAngle = 70f;

        public bool limitDetectionDistance;

        [Tooltip("Length in meters in case distance limit detection is activated")]
        public float maxDistance = 1.5f;

        [Tooltip("Distance in meters that triggers the proximity event")]
        public float proximityThreshold = 1f;

        [Tooltip("Transform which will be used as the starting point of forward vision")]
        public Transform scanDirection;

        [Tooltip("Defines if actions based on visibility should be triggered, e.g. speech if defined. Deactivate for slight performance improvements.")]
        public bool reportVisibility = true;

        [Header("Events")] public UnityEvent onDetected;
        public UnityEvent onLostSight;
        public UnityEvent onProximity;
    }
}