using Newtonsoft.Json;
using System;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class ObjectSpec
    {
        public enum PivotType
        {
            Bottom_Front_Left = 0,
            Bottom_Front_Center = 1,
            Center = 2,
            Bottom_Center = 3
        }

        [HideInInspector] public string objectKey;
        [JsonIgnore] [HideInInspector] public GameObject gameObject;
        public Vector3 position;
        public Vector3 rotation;
        [DefaultValue("Vector3.one")] public Vector3 scale = Vector3.one;
        [DefaultValue(1)] public byte width = 1;
        [DefaultValue(1)] public byte height = 1;

        [Tooltip("Moves object to the ceiling.")]
        public bool atCeiling;

        [Tooltip("Snap object to the next nearby surface.")]
        public bool pinToSide;

        [Tooltip("Snaps an object to the side and not to the front, e.g. when attaching directional arrows to the wall.")]
        public bool snapSideways;

        [Tooltip("Disables spawning on top of the object at the main orientation side.")]
        public bool standalone;

        [Tooltip("Disables spawning on top of the object on all sides.")]
        public bool standaloneAllSides;

        [Tooltip("Replaces shaders with included ones that support stencil operations.")] [DefaultValue(true)]
        public bool adjustMaterials = true;

        public PivotType pivotType = PivotType.Bottom_Front_Left;

        [NonSerialized] public SingleBehaviors behaviors;

        public ObjectSpec()
        {
        }

        public ObjectSpec(string objectKey) : this()
        {
            this.objectKey = objectKey;
        }

        public bool IsDefault()
        {
            if (position != Vector3.zero) return false;
            if (rotation != Vector3.zero) return false;
            if (scale != Vector3.one) return false;
            if (height != 1) return false;
            if (width != 1) return false;
            if (atCeiling) return false;
            if (pinToSide) return false;
            if (standalone) return false;
            if (standaloneAllSides) return false;
            if (snapSideways) return false;
            if (!adjustMaterials) return false;
            if (pivotType != PivotType.Bottom_Front_Left) return false;

            return true;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(objectKey))
            {
                return $"Spec for soft-referenced {objectKey} (pivot: {pivotType}, pin-to-side: {pinToSide})";
            }
            return $"Spec for {gameObject} (pivot: {pivotType}, pin-to-side: {pinToSide})";
        }
    }
}