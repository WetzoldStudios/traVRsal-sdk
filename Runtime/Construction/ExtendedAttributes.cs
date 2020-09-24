using UnityEngine;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [DisallowMultipleComponent]
    public class ExtendedAttributes : MonoBehaviour
    {
        [Tooltip("Object does not use any logic components, enabling further performance enhancements.")]
        public bool environment = false;

        [Tooltip("Object should not block agents.")]
        public bool nonBlocking = false;

        [Tooltip("Provides walkable ground to the player.")]
        public bool climbable = false;

        [Tooltip("Object is an item that can be carried.")]
        public bool carriable = false;

        [Tooltip("Object is a flat wall of width 1 that can be hidden without visual pop-in effects when walking through a transition.")]
        public bool portalHole = false;

        public bool proportionateScaling = false;
        public Direction initialDirection = Direction.South;

        [Tooltip("Additional object attributes.")]
        public ObjectSpec spec;
    }
}