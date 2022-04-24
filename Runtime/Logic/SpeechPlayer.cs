using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Speech Player")]
    public class SpeechPlayer : MonoBehaviour
    {
        [Header("Content")] public AudioClip clip;
        public string subtitle;
        public string speaker;

        [Header("Configuration")] public float pauseBefore;
        public float pauseAfter;
        public float lengthReduction;
        public float volumeScale = 1f;

        [Header("Events")] public UnityEvent onDone;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponentInParent<AudioSource>();
        }

        public void Trigger()
        {
            StartCoroutine(PlayAudio());
        }

        private IEnumerator PlayAudio()
        {
            yield return new WaitForSeconds(pauseBefore);

            if (clip != null)
            {
                if (_audioSource != null) _audioSource.PlayOneShot(clip, volumeScale);
                yield return new WaitForSeconds(clip.length - lengthReduction);
            }

            yield return new WaitForSeconds(pauseAfter);

            onDone?.Invoke();
        }
    }
}