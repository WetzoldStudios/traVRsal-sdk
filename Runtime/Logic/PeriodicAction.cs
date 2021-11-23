using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Periodic Action")]
    public class PeriodicAction : MonoBehaviour
    {
        [Header("Timing")] public Vector2 initialDelay = new Vector2(1f, 2f);
        public Vector2 interval = new Vector2(3f, 4f);

        [Tooltip("Keep interval the same once initially determined or determine new after each action")]
        public bool fixedInterval = true;

        [Header("Actions")] public UnityEvent<bool> performAction;

        private float _finalInterval;
        private float _nextAction;
        private bool _state;

        private void Start()
        {
            _finalInterval = Random.Range(interval.x, interval.y);
            _nextAction = Time.time + Random.Range(initialDelay.x, initialDelay.y) + (fixedInterval ? _finalInterval : Random.Range(interval.x, interval.y));
        }

        private void Update()
        {
            if (!(Time.time > _nextAction)) return;

            _state = !_state;
            _nextAction = Time.time + (fixedInterval ? _finalInterval : Random.Range(interval.x, interval.y));
            performAction?.Invoke(_state);
        }
    }
}