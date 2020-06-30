using UnityEngine;

namespace traVRsal.SDK
{
    public class ExtendedAttributes : MonoBehaviour
    {
        public bool nonBlocking = false;
        public bool climbable = false;
        public bool portalHole = false;
        public BasicEntity.Direction initialDirection = BasicEntity.Direction.SOUTH;
    }
}