using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Grabbable")]
    public class Grabbable : ExecutorConfig
    {
        [Header("Configuration")] [Tooltip("Return item to play area if it becomes unreachable, e.g. if player throws it away.")]
        public bool autoReturn;

        [Header("On Grab")] public bool hideHand = true;

        [Space] public UnityEvent onGrab;

        [Header("On Drop")] public bool activateGravity = true;
        public bool deactivateKinematic = true;

        [Space] public UnityEvent onDrop;
    }
}