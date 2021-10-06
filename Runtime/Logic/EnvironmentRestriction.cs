using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Environment Restriction")]
    public class EnvironmentRestriction : MonoBehaviour
    {
        public enum Mode
        {
            Disable = 0,
            Destroy = 1,
            EventsOnly = 2
        }

        public Mode mode = Mode.Disable;

        [Header("Checks")] public bool requirePlatform;
        public RuntimePlatform[] platforms;

        public bool requireHighMemory;
        public bool requireHighPerformance;

        [Header("Events")] public UnityEvent onValid;
        public UnityEvent onInvalid;

        private void Awake()
        {
            bool valid = true;

            IEnvironment[] contexts = GetComponentsInParent<IEnvironment>(true);
            if (contexts.Length > 0)
            {
                if (requireHighMemory && contexts[0].GetEnvironmentInfo().lowMemoryMode) valid = false;
                if (requireHighPerformance && contexts[0].GetEnvironmentInfo().lowQualityMode) valid = false;
            }
            if (requirePlatform && !platforms.Any(p => p == Application.platform)) valid = false;

            if (valid)
            {
                onValid?.Invoke();
                return;
            }
            onInvalid?.Invoke();

            switch (mode)
            {
                case Mode.Disable:
                    gameObject.SetActive(false);
                    break;

                case Mode.Destroy:
                    Destroy(gameObject);
                    return;
            }
        }
    }
}