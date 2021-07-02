using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Spawner")]
    public class Spawner : ExecutorConfig
    {
        public enum Mode
        {
            Periodic = 0,
            MaxAmount = 1
        }

        public Mode mode = Mode.Periodic;
        public string[] objects;
        public float delay = 2f;
        public bool randomRotation = true;
    }
}