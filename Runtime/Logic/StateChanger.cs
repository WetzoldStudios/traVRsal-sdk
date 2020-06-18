using UnityEngine;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    public class StateChanger : ExecutorConfig
    {
        public string key = "alt";
        public float duration = 5f;
        public List<Behaviour> enabledComponents;
        public List<Behaviour> disabledComponents;
        public List<MaterialReference> materialSlots;
    }
}