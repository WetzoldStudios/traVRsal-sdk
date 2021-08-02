using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Grabbable")]
    public class Grabbable : ExecutorConfig
    {
        [Header("On Grab")] public bool hideHand = true;

        [Space] public List<GameObject> enabledObjectsOnGrab;
        public List<GameObject> disabledObjectsOnGrab;
        public List<Behaviour> enabledComponentsOnGrab;
        public List<Behaviour> disabledComponentsOnGrab;

        [Header("On Drop")] public bool activateGravity = true;
        public bool deactivateKinematic = true;

        [Space] public List<GameObject> enabledObjectsOnDrop;
        public List<GameObject> disabledObjectsOnDrop;
        public List<Behaviour> enabledComponentsOnDrop;
        public List<Behaviour> disabledComponentsOnDrop;
    }
}