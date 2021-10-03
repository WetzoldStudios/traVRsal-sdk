using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Periodic Animator")]
    public class PeriodicAnimator : MonoBehaviour
    {
        [Header("Configuration")] public float initialRandomDelay = 3f;
        public float interval = 6f;
        public string triggerKey1;
        public string triggerKey2;
        public float trigger2Offset = 2f;

        [Header("Sounds")] public AudioSource sound1;
        public float sound1Delay;
        public float sound1Duration;
        public AudioSource sound2;
        public float sound2Delay;
        public float sound2Duration;

        private Animator _animator;
        private float _nextTrigger;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _nextTrigger = Time.time + Random.Range(0, initialRandomDelay);
        }

        private void Update()
        {
            if (!(Time.time > _nextTrigger)) return;

            _nextTrigger = Time.time + interval;
            if (!string.IsNullOrEmpty(triggerKey1)) StartCoroutine(TriggerAnimation(triggerKey1, 0));
            if (!string.IsNullOrEmpty(triggerKey2)) StartCoroutine(TriggerAnimation(triggerKey2, trigger2Offset));

            if (sound1 != null) StartCoroutine(PlaySound(sound1, sound1Delay, sound1Duration));
            if (sound2 != null) StartCoroutine(PlaySound(sound2, sound2Delay, sound2Duration));
        }

        private IEnumerator TriggerAnimation(string key, float delay)
        {
            yield return new WaitForSeconds(delay);
            _animator.SetTrigger(key);
        }

        private static IEnumerator PlaySound(AudioSource sound, float delay, float duration)
        {
            sound.Stop();
            yield return new WaitForSeconds(delay);
            sound.Play();

            if (duration > 0)
            {
                yield return new WaitForSeconds(duration);
                sound.Stop();
            }
        }
    }
}