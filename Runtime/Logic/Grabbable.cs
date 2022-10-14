using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Grabbable")]
    public class Grabbable : ExecutorConfig
    {
        [Header("Configuration")] [Tooltip("Return item to play area if it becomes unreachable, e.g. if player throws it away.")]
        public bool autoReturn;

        [Tooltip("Return item either to the ground in front of the player or to its original position.")]
        public bool returnToOriginalPosition;

        [Header("On Grab")] [Tooltip("Add this item to the permanent inventory. It cannot be dropped anymore but only switched.")]
        public bool addToInventory;

        public bool hideHand = true;

        [Header("On Drop")] public bool activateGravity = true;
        public bool deactivateKinematic = true;

        [Header("Events")] public UnityEvent onGrab;
        public UnityEvent onDrop;
        public UnityEvent onTouchHead;

        public void TriggerAutoReturn()
        {
            ISpawner spawner = GetComponentInParent<ISpawner>();
            spawner?.TriggerAutoReturn(gameObject);
        }
    }
}