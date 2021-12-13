using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/State Changer")]
    public class StateChanger : ExecutorConfig
    {
        public string key = "alt";
        public float duration = 5f;
        public List<MaterialReference> materialSlots;

        [Header("Events")] public UnityEvent onIntoState;
        public UnityEvent onOutOfState;
        public UnityEvent<bool> onStateChange;
    }
}