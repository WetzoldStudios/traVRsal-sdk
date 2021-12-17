using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

        [Tooltip("Delay in seconds")] [FormerlySerializedAs("delay")]
        public float duration = 2f;

        [Header("Events")] public UnityEvent onCompletion;

        private ITime _timer;

        private void OnEnable()
        {
            if (mode == Mode.Automatic) Trigger(duration);
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            Trigger(duration);
        }

        public void Trigger(float durationOverride)
        {
            _timer ??= GetComponentsInParent<ITime>(true).FirstOrDefault();
            if (_timer == null) return;

            // use central timer since coroutines are canceled when objects become inactive
            _timer.Delay(durationOverride, () => onCompletion?.Invoke());
        }
    }
}