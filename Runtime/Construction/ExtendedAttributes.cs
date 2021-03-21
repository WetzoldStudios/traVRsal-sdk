using System;
using UnityEngine;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    [DisallowMultipleComponent]
    public class ExtendedAttributes : MonoBehaviour
    {
        [Tooltip("Object does not move and does not use any logic components, enabling further performance enhancements.")]
        public bool environment;

        [Tooltip("Object should not block agents.")]
        public bool nonBlocking;

        [Tooltip("Object provides walkable ground to the player.")]
        public bool climbable;

        [Tooltip("Object is an item that can be carried.")]
        public bool carriable;

        [Tooltip("Object is a flat wall of width 1 that can be hidden without visual pop-in effects when walking through a transition.")]
        public bool portalHole;

        public bool proportionateScaling;
        public Direction initialDirection = Direction.South;

        [Tooltip("Additional object attributes")]
        public ObjectSpec spec;
    }
}