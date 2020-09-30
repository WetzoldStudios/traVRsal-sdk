using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    public class AnimatorTrigger : MonoBehaviour
    {
        [Header("Configuration")] public string triggerKey;
        public float interval = 5f;
        public float initialRandomDelay = 3f;

        [Header("Sounds")] public AudioSource sound1;
        public float sound1Delay;
        public float sound1Duration;
        public AudioSource sound2;
        public float sound2Delay;
        public float sound2Duration;

        private Animator animator;
        private float nextTrigger;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            nextTrigger = Time.time + Random.Range(0, initialRandomDelay);
        }

        private void Update()
        {
            if (Time.time > nextTrigger)
            {
                animator.SetTrigger(triggerKey);
                nextTrigger = Time.time + interval;

                if (sound1 != null) StartCoroutine(PlaySound(sound1, sound1Delay, sound1Duration));
                if (sound2 != null) StartCoroutine(PlaySound(sound2, sound2Delay, sound2Duration));
            }
        }

        private IEnumerator PlaySound(AudioSource sound, float delay, float duration)
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