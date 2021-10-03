using System;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/World State Reactor")]
    public class WorldStateReactor : MonoBehaviour, IWorldStateReactor
    {
        [Tooltip("Components to activate when world has finished loading.")] [Obsolete]
        public Behaviour[] components;

        public UnityEvent onFinishedLoading;

        [Tooltip("Components to inform about zone changes. Requires WorldStateReactor components on these.")]
        public Component[] inform;

        public void ZoneChange(Zone zone, bool isCurrent)
        {
            inform?.ForEach(i => ((IWorldStateReactor) i).ZoneChange(zone, isCurrent));
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            onFinishedLoading?.Invoke();

            for (int i = 0; i < components.Length; i++)
            {
                components[i].enabled = true;
            }
        }

        private void Start()
        {
            // in to enable activation/deactivation
        }
    }
}