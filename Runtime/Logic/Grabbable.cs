using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Grabbable")]
    public class Grabbable : ExecutorConfig
    {
        [Header("On Grab")]
        public bool hideHand = true;

        [Header("On Drop")]
        public bool activateGravity = true;
        public bool deactivateKinematic = true;
    }
}