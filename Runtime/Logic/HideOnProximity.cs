using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Hide on Proximity")]
    public class HideOnProximity : MonoBehaviour
    {
        [Header("Static References")] public Transform trackedObject;

        [Tooltip("Object which the distance is measured against. Defaults to current object.")]
        public Transform referenceObject;

        [Tooltip("Object which visibility should be affected.")]
        public GameObject managedObject;

        [Header("Configuration")] public float distance = 0.5f;

        private void Start()
        {
            if (referenceObject == null) referenceObject = transform;
        }

        private void Update()
        {
            if (trackedObject == null || referenceObject == null || managedObject == null) return;

            float measuredDistance = Mathf.Abs((trackedObject.position - referenceObject.position).magnitude);
            bool visible = measuredDistance >= distance;
            if (managedObject.activeSelf != visible) managedObject.SetActive(visible);
        }
    }
}