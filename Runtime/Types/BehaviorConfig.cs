using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public abstract class BehaviorConfig
    {
        [HideInInspector]
        public string[] objectKeys;
        [HideInInspector]
        public SDKUtil.ColliderType autoAddCollider = SDKUtil.ColliderType.Box;
    }
}