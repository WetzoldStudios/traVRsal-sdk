using UnityEngine;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    public class ZoneRestriction : ExecutorConfig
    {
        public List<GameObject> enabledGameObjects;
        public List<Behaviour> enabledComponents;
    }
}