using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Destroy")]
    public class Destroy : MonoBehaviour
    {
        public enum Mode
        {
            Automatic = 0,
            Manual = 1
        }

        public Mode mode = Mode.Automatic;

        [Tooltip("Delay for destruction in seconds")]
        public float delay = 5f;

        [Tooltip("Flag if full piece should be destroyed or only the hierarchy from this object and below")]
        public bool partial;

        [Space] public UnityEvent onDestruction;

        private ISpawner _spawner;

        private void Start()
        {
            _spawner = GetComponentInParent<ISpawner>();
        }

        private void OnEnable()
        {
            if (mode == Mode.Automatic) StartCoroutine(DoTrigger(delay));
        }

        private IEnumerator DoTrigger(float delayOverride)
        {
            yield return new WaitForSeconds(delayOverride);
            onDestruction?.Invoke();
            _spawner.Destruct(gameObject, partial);
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            StartCoroutine(DoTrigger(delay));
        }

        public void Trigger(float delayOverride)
        {
            StartCoroutine(DoTrigger(delayOverride));
        }
    }
}