using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Show Only When Facing Player")]
    [RequireComponent(typeof(Renderer))]
    public class OnlyFacingPlayer : ExecutorConfig
    {
        public bool invert;
    }
}