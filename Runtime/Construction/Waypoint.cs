using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Waypoint : MonoBehaviour
    {
        public enum Type
        {
            Default = 0,
            Locomotion = 1
        }

        public Type type = Type.Default;
        public string key;
        public float pause;

        public Waypoint()
        {
        }

        public override string ToString()
        {
            return $"Waypoint {key} ({type})";
        }
    }
}