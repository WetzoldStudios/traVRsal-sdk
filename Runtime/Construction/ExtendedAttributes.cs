using System;
using UnityEngine;

namespace traVRsal.SDK
{
    public enum ScalingMode
    {
        Auto = 0,
        XZ = 1,
        XYZ = 2,
        None = 3
    }

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

        [Tooltip("Object will control visibility of portal at same location")]
        public bool door;

        [Tooltip("Object is a flat wall of width 1 that can be hidden without visual pop-in effects when walking through a transition")]
        public bool portalHole;

        [Tooltip("Ignore object when calculating center point of zone inside the studio")]
        public bool editorCamIgnore;

        [Tooltip("Scale object only in XZ direction or also along Y axis. Scenery will per default not scale at all (Auto).")]
        public ScalingMode scalingMode;

        [Tooltip("Additional object attributes")]
        public ObjectSpec spec;

        // cache structures
        [NonSerialized] public Behaviour portal;
        private IPortalAction _portalAction;

        public void SetDoorOpen(bool state)
        {
            _portalAction ??= GetComponentsInParent<IPortalAction>(true)[0];
            _portalAction.SetPortalState(state);
        }
    }
}