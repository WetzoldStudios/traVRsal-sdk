using UnityEngine;

namespace traVRsal.SDK
{
    [RequireComponent(typeof(Renderer))]
    public class OnlyFacingPlayer : ExecutorConfig
    {
        public bool invert;
    }
}