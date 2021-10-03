using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Grabbable")]
    public class Grabbable : ExecutorConfig
    {
        [Header("On Grab")] public bool hideHand = true;

        [Space] [Obsolete] public List<GameObject> enabledObjectsOnGrab;
        [Obsolete] public List<GameObject> disabledObjectsOnGrab;
        [Obsolete] public List<Behaviour> enabledComponentsOnGrab;
        [Obsolete] public List<Behaviour> disabledComponentsOnGrab;
        public UnityEvent onGrab;

        [Header("On Drop")] public bool activateGravity = true;
        public bool deactivateKinematic = true;

        [Space] [Obsolete] public List<GameObject> enabledObjectsOnDrop;
        [Obsolete] public List<GameObject> disabledObjectsOnDrop;
        [Obsolete] public List<Behaviour> enabledComponentsOnDrop;
        [Obsolete] public List<Behaviour> disabledComponentsOnDrop;
        public UnityEvent onDrop;
    }
}