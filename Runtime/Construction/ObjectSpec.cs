using Newtonsoft.Json;
using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ObjectSpec
    {
        public enum PivotType
        {
            BOTTOM_FRONT_LEFT, BOTTOM_FRONT_CENTER, CENTER, BOTTOM_CENTER
        }

        [HideInInspector]
        public string objectKey;
        [JsonIgnore]
        public GameObject gameObject;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.one;
        public int height = 1;
        public bool atCeiling = false;
        public bool pinToSide = false;
        public bool snapSideways = false;
        public bool adjustMaterials = true;
        public PivotType pivotType = PivotType.BOTTOM_FRONT_LEFT;

        [NonSerialized]
        public SingleBehaviors behaviors;

        public ObjectSpec() { }

        public ObjectSpec(string objectKey) : this()
        {
            this.objectKey = objectKey;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(objectKey))
            {
                return $"Spec for soft-referenced {objectKey} (pivot: {pivotType}, pin-to-side: {pinToSide})";
            }
            else
            {
                return $"Spec for {gameObject} (pivot: {pivotType}, pin-to-side: {pinToSide})";
            }
        }
    }
}