using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Story Player")]
    public class StoryPlayer : MonoBehaviour, IWorldStateReactor
    {
        public enum Mode
        {
            Automatic = 0,
            Manual = 1
        }

        public Mode mode;
        public TextAsset file;

        [Header("Events")] [Tooltip("Internal usage. Overridden automatically during packaging.")]
        public UnityEvent onTrigger;

        public UnityEvent onStart;
        public UnityEvent onCompletion;

        private bool _triggered;

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            if (_triggered) return;
            _triggered = true;

            onTrigger?.Invoke();
            onStart?.Invoke();
        }

        [ContextMenu("Mark Completed")]
        public void SignalDone()
        {
            onCompletion?.Invoke();
        }

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            if (mode == Mode.Automatic) Trigger();
        }
    }
}