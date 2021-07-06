using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Only Facing Player")]
    [RequireComponent(typeof(Renderer))]
    public class OnlyFacingPlayer : ExecutorConfig
    {
        public bool invert;
    }
}