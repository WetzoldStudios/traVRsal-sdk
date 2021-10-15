using System;
using System.Collections.Generic;
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

        [Header("Action")] [Obsolete] public List<GameObject> toggleObjects;
        [Obsolete] public List<Behaviour> toggleComponents;
        [Obsolete] public List<Collider> toggleColliders;

        public UnityEvent<bool> performAction;

        private float _finalInterval;
        private float _nextAction;
        private bool _state;

        private void Start()
        {
            _finalInterval = Random.Range(interval.x, interval.y);
            _nextAction = Time.time + Random.Range(initialDelay.x, initialDelay.y) + (fixedInterval ? _finalInterval : Random.Range(interval.x, interval.y));

            if (toggleObjects != null && toggleObjects.Count > 0) EDebug.LogWarning($"Obsolete logic usage on {gameObject}");
            if (toggleComponents != null && toggleComponents.Count > 0) EDebug.LogWarning($"Obsolete logic usage on {gameObject}");
            if (toggleColliders != null && toggleColliders.Count > 0) EDebug.LogWarning($"Obsolete logic usage on {gameObject}");
        }

        private void Update()
        {
            if (!(Time.time > _nextAction)) return;

            _state = !_state;
            _nextAction = Time.time + (fixedInterval ? _finalInterval : Random.Range(interval.x, interval.y));
            performAction?.Invoke(_state);

            toggleObjects.ForEach(go => go.SetActive(!go.activeSelf));
            toggleComponents.ForEach(b => b.enabled = !b.enabled);
            toggleColliders.ForEach(b => b.enabled = !b.enabled);
        }
    }
}