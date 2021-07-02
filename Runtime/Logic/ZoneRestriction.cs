using UnityEngine;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Zone Restriction")]
    public class ZoneRestriction : ExecutorConfig
    {
        [Tooltip("Game objects to enable if the player enters this zone.")]
        public List<GameObject> enabledGameObjects;

        [Tooltip("Components to enable if the player enters this zone.")]
        public List<Behaviour> enabledComponents;

        [Tooltip("Components to inform about the zone change.")] [SerializeReference]
        public Component[] informedComponents;

        [Tooltip("Ignore zone restriction if object is contained in the first zone.")]
        public bool allowInIntro = true;
    }
}