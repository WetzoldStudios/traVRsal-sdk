using System;
using UnityEngine;
using static traVRsal.SDK.BasicEntity;

namespace traVRsal.SDK
{
    [Serializable]
    [DisallowMultipleComponent]
    public class ExtendedAttributes : MonoBehaviour
    {
        [Tooltip("Object is static, does not move, is not an agent and does not use logic components, enabling further performance enhancements.")]
        public bool environment;

        [Tooltip("Object should not block agents")]
        public bool nonBlocking;

        [Tooltip("Object provides walkable ground to the player")]
        public bool climbable;

        [Tooltip("Object is an item that can be carried")]
        public bool carriable;

        [Tooltip("Object is a flat wall of width 1 that can be hidden without visual pop-in effects when walking through a transition")]
        public bool portalHole;

        [Tooltip("Scale object also along Y axis")]
        public bool proportionateScaling;

        [Tooltip("Ignore object when calculating center point of zone inside the studio")]
        public bool editorCamIgnore;

        public Direction initialDirection = Direction.South;

        [Tooltip("Additional object attributes")]
        public ObjectSpec spec;
    }
}