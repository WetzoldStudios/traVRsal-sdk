using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ExecutorConfig : MonoBehaviour, IExecutorConfig
    {
        private int uniqueId = 0;

        void Start() { }

        public int GetUniqueId()
        {
            if (uniqueId == 0) uniqueId = ThreadSafeRandom.ThisThreadsRandom.Next();

            return uniqueId;
        }
    }
}