using UnityEngine;

namespace traVRsal.SDK
{
    public class WorldStateReactor : MonoBehaviour, IWorldStateReactor
    {
        [Tooltip("Components to activate when world has finished loading.")]
        public Behaviour[] components;

        [Tooltip("Components to inform about zone changes.")]
        public Component[] inform;

        public void ZoneChange(Zone zone, bool isCurrent)
        {
            inform?.ForEach(i => ((IWorldStateReactor) i).ZoneChange(zone, isCurrent));
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
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