using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    public class TimeManipulator : MonoBehaviour
    {
        public enum OperationMode
        {
            User_Driven,
            Auto_Deactivate
        }

        public float slowTimeValue = 0.05f;
        public float duration = 3f;
        public float cooldown = 5f;

        [HideInInspector] public OperationMode mode = OperationMode.User_Driven;
        [HideInInspector] public float defaultPhysicsTimeStep;

        private float lastUsage;
        private AudioSource audioSource;

        private void Start()
        {
            switch (mode)
            {
                case OperationMode.Auto_Deactivate:
                    StartCoroutine(StartTimeEffect());
                    break;

                case OperationMode.User_Driven:
                    defaultPhysicsTimeStep = Time.fixedDeltaTime;
                    audioSource = GetComponent<AudioSource>();
                    if (audioSource != null && !audioSource.enabled) audioSource = null;
                    break;
            }
        }

        public void Fire()
        {
            if (Time.time < lastUsage + cooldown) return;

            lastUsage = Time.time;

            GameObject go = new GameObject(SDKUtil.AUTO_GENERATED + "TimeReset");
            TimeManipulator tm = go.AddComponent<TimeManipulator>();
            tm.mode = OperationMode.Auto_Deactivate;
            tm.defaultPhysicsTimeStep = defaultPhysicsTimeStep;
            tm.slowTimeValue = slowTimeValue;
            tm.duration = duration;
            tm.cooldown = cooldown;

            if (audioSource != null) audioSource.Play();
        }

        private IEnumerator StartTimeEffect()
        {
            Time.timeScale = slowTimeValue;
            Time.fixedDeltaTime *= slowTimeValue;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultPhysicsTimeStep; // do not multiplicate back as other scripts might also fumble with time, e.g. moving platform

            if (mode == OperationMode.Auto_Deactivate) Destroy(gameObject);
        }
    }
}