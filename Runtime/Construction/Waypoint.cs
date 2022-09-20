using System;
using System.Linq;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class Waypoint : MonoBehaviour
    {
        public enum Type
        {
            Default = 0,
            Locomotion = 1
        }

        public Type type = Type.Default;
        public string key;
        public float pause;

        private Transform _normalized;

        public Waypoint()
        {
        }

        public Transform GetTransform(bool normalizePivot)
        {
            if (!normalizePivot) return transform;

            // return parent in case non-center pivot is required
            if (_normalized == null)
            {
                NamedIdentifier nid = GetComponentsInParent<NamedIdentifier>(true).FirstOrDefault(ni => ni.type == NamedIdentifier.IdentifierType.AutoParent);
                _normalized = nid != null ? nid.transform : transform;
            }
            return _normalized;
        }

        public override string ToString()
        {
            return $"Waypoint '{key}' ({type})";
        }
    }
}