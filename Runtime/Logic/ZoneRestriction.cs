using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Zone Restriction")]
    public class ZoneRestriction : ExecutorConfig
    {
        [Tooltip("Reacts only if a specifically named zone becomes active, otherwise the one where the component is placed.")]
        public string filter;

        [Tooltip("Ignore zone restriction if object is contained in the first zone.")]
        public bool allowInIntro = true;

        [Tooltip("Components to inform about the zone change.")] [SerializeReference]
        public Component[] informedComponents;

        [Header("Events")] [Tooltip("Anytime the zone becomes active, also during initial loading. Can fire multiple times.")]
        public UnityEvent onActive;

        [Tooltip("Anytime the zone becomes active and was inactive before. Fired only after initial loading.")]
        public UnityEvent onBecomeActive;

        [Tooltip("Anytime the zone becomes inactive, also during initial loading. Can fire multiple times.")]
        public UnityEvent onInactive;

        [Tooltip("Anytime the zone becomes inactive and was active before. Fired only after initial loading.")]
        public UnityEvent onBecomeInactive;

        [Tooltip("Fired whenever a zone is changed. Boolean parameter will be true is changed to this zone.")]
        public UnityEvent<bool> onZoneChange;
    }
}