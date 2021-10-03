using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Time Manipulator")]
    public class TimeManipulator : MonoBehaviour
    {
        public enum OperationMode
        {
            UserDriven = 0,
            AutoDeactivate = 1
        }

        public float slowTimeValue = 0.05f;
        public float duration = 3f;
        public float cooldown = 5f;

        [HideInInspector] public OperationMode mode = OperationMode.UserDriven;
        [HideInInspector] public float defaultPhysicsTimeStep;

        private float _lastUsage;
        private AudioSource _audioSource;

        private void Start()
        {
            switch (mode)
            {
                case OperationMode.AutoDeactivate:
                    StartCoroutine(StartTimeEffect());
                    break;

                case OperationMode.UserDriven:
                    defaultPhysicsTimeStep = Time.fixedDeltaTime;
                    _audioSource = GetComponent<AudioSource>();
                    if (_audioSource != null && !_audioSource.enabled) _audioSource = null;
                    break;
            }
        }

        public void Fire()
        {
            if (Time.time < _lastUsage + cooldown) return;

            _lastUsage = Time.time;

            GameObject go = new GameObject(SDKUtil.AUTO_GENERATED + "TimeReset");
            TimeManipulator tm = go.AddComponent<TimeManipulator>();
            tm.mode = OperationMode.AutoDeactivate;
            tm.defaultPhysicsTimeStep = defaultPhysicsTimeStep;
            tm.slowTimeValue = slowTimeValue;
            tm.duration = duration;
            tm.cooldown = cooldown;

            if (_audioSource != null) _audioSource.Play();
        }

        private IEnumerator StartTimeEffect()
        {
            Time.timeScale = slowTimeValue;
            Time.fixedDeltaTime *= slowTimeValue;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultPhysicsTimeStep; // do not multiply back as other scripts might also fumble with time, e.g. moving platform

            if (mode == OperationMode.AutoDeactivate) Destroy(gameObject);
        }
    }
}