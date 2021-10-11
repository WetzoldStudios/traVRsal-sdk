using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Delay")]
    public class Delay : MonoBehaviour
    {
        public enum Mode
        {
            Automatic = 0,
            Manual = 1
        }

        public Mode mode = Mode.Automatic;
        [Tooltip("Delay in seconds")] public float delay = 2f;

        [Space] public UnityEvent onCompletion;

        private void OnEnable()
        {
            if (mode == Mode.Automatic) StartCoroutine(DoTrigger(delay));
        }

        private IEnumerator DoTrigger(float delay)
        {
            yield return new WaitForSeconds(delay);
            onCompletion?.Invoke();
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            StartCoroutine(DoTrigger(delay));
        }

        public void Trigger(float delay)
        {
            StartCoroutine(DoTrigger(delay));
        }
    }
}