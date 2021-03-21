using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    public class PeriodicAction : MonoBehaviour
    {
        [Header("Timing")] public Vector2 initialDelay = new Vector2(1f, 2f);
        public Vector2 interval = new Vector2(3f, 4f);
        public bool fixedInterval = true;

        [Header("Action")] public List<GameObject> toggleObjects;
        public List<Behaviour> toggleComponents;
        public List<Collider> toggleColliders;

        private float finalInterval;
        private float nextAction;

        private void Start()
        {
            finalInterval = Random.Range(interval.x, interval.y);
            nextAction = Time.time + Random.Range(initialDelay.x, initialDelay.y) + (fixedInterval ? finalInterval : Random.Range(interval.x, interval.y));
        }

        private void Update()
        {
            if (Time.time > nextAction)
            {
                nextAction = Time.time + (fixedInterval ? finalInterval : Random.Range(interval.x, interval.y));

                toggleObjects.ForEach(go => go.SetActive(!go.activeSelf));
                toggleComponents.ForEach(b => b.enabled = !b.enabled);
                toggleColliders.ForEach(b => b.enabled = !b.enabled);
            }
        }
    }
}