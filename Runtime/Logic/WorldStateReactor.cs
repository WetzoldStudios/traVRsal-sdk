using UnityEngine;

namespace traVRsal.SDK
{
    public class WorldStateReactor : MonoBehaviour, IWorldStateReactor
    {
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
        }
    }
}