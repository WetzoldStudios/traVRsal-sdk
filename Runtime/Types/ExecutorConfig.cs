using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ExecutorConfig : MonoBehaviour, IExecutorConfig
    {
        public enum UserAction
        {
            Touch = 0,
            Trigger = 1
        }

        private int uniqueId;

        private void Start()
        {
        }

        public int GetUniqueId()
        {
            if (uniqueId == 0) uniqueId = ThreadSafeRandom.ThisThreadsRandom.Next();

            return uniqueId;
        }
    }
}