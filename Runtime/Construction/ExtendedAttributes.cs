using UnityEngine;

namespace traVRsal.SDK
{
    [DisallowMultipleComponent]
    public class ExtendedAttributes : MonoBehaviour
    {
        public bool environment = false;
        public bool nonBlocking = false;
        public bool climbable = false;
        public bool portalHole = false;
        public BasicEntity.Direction initialDirection = BasicEntity.Direction.SOUTH;
        public ObjectSpec spec;
    }
}