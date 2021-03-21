using UnityEngine;

namespace traVRsal.SDK
{
    public class WorldStateReactor : MonoBehaviour, IWorldStateReactor
    {
        [Tooltip("Components to activate when world has finished loading.")]
        public Behaviour[] components;

        public void FinishedLoading()
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