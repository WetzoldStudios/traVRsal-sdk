using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/World State Reactor")]
    public class WorldStateReactor : MonoBehaviour, IWorldStateReactor
    {
        [Header("Events")] public UnityEvent onFinishedLoading;

        [Tooltip("Components to inform about zone changes. Requires WorldStateReactor components on these.")]
        public Component[] inform;

        public void ZoneChange(Zone zone, bool isCurrent)
        {
            inform?.ForEach(i => ((IWorldStateReactor) i).ZoneChange(zone, isCurrent));
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            if (enabled) onFinishedLoading?.Invoke();
        }

        private void Start()
        {
            // in to enable activation/deactivation
        }
    }
}