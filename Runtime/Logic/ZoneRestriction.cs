using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Zone Restriction")]
    public class ZoneRestriction : ExecutorConfig
    {
        [Tooltip("Components to inform about the zone change.")] [SerializeReference]
        public Component[] informedComponents;

        [Tooltip("Ignore zone restriction if object is contained in the first zone.")]
        public bool allowInIntro = true;

        [Header("Events")] public UnityEvent onActive;
        public UnityEvent onInactive;
        public UnityEvent<bool> onZoneChange;
    }
}