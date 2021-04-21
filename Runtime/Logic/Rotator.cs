using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Rotator : MonoBehaviour, IVariableReactor, IWorldStateReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;

        [Tooltip("Degrees per second in manual mode. Total degrees otherwise. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 degrees = new Vector2(10f, 10f);

        [Tooltip("Local axis around which the object should be rotated, e.g. (0,1,0) for Y.")]
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Incremental;

        [Header("Timings")] public Vector2 initialDelay;
        public Vector2 duration = new Vector2(2f, 2f);
        public Vector2 onDelay;
        public Vector2 offDelay;

        [Header("Audio")] [Tooltip("Sound to play when rotation is started.")]
        public AudioSource audio;

        private float finalDegrees;
        private float finalInitialDelay;
        private float finalDuration;
        private float finalOnDelay;
        private float finalOffDelay;

        private Quaternion originalRotationQ;
        private Vector3 originalRotation;
        private bool changedOnce;
        private float startTime;
        private bool initDone;
        private bool loadingDone;
        private Tween curTween;

        private void Start()
        {
            originalRotationQ = transform.localRotation;
            originalRotation = originalRotationQ.eulerAngles;

            finalDegrees = Random.Range(degrees.x, degrees.y);
            finalInitialDelay = Random.Range(initialDelay.x, initialDelay.y);
            finalDuration = Random.Range(duration.x, duration.y);
            finalOnDelay = Random.Range(onDelay.x, onDelay.y);
            finalOffDelay = Random.Range(offDelay.x, offDelay.y);
        }

        private void OnEnable()
        {
            if (!loadingDone) return;
            if (initDone)
            {
                if (curTween != null && !curTween.IsComplete()) curTween.Play();
                return;
            }

            // needed for support of initialDelay, since any WaitForSeconds will be interrupted during loading when GO becomes inactive
            startTime = Time.time + finalInitialDelay;
        }

        private void OnDisable()
        {
            if (curTween == null) return;

            if (curTween.IsPlaying()) curTween.Pause();
        }

        private void Update()
        {
            if (initDone || mode != Mode.Manual) return;
            if (startTime > 0 && Time.time > startTime)
            {
                SetupManual();
                initDone = true;
            }
        }

        private void SetupManual()
        {
            if (finalDegrees == 0) return;

            Sequence s = DOTween.Sequence();
            s.PrependInterval(finalOnDelay);
            s.AppendCallback(PlayAudio); // OnPlay is only called once in a sequence
            s.Append(transform.DOLocalRotate(originalRotation + axis * finalDegrees, finalDuration).SetEase(easeType));
            s.AppendInterval(finalOffDelay);
            s.SetLoops(loop ? -1 : 0, loopType);

            curTween = s;
        }

        private void PlayAudio()
        {
            if (audio != null) audio.Play();
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                curTween = transform.DOLocalRotate(originalRotation + axis * finalDegrees, finalDuration).SetDelay(finalOnDelay + (changedOnce ? 0f : finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }
            else
            {
                curTween = transform.DOLocalRotateQuaternion(originalRotationQ, finalDuration).SetDelay(finalOffDelay + (changedOnce ? 0f : finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }

        public void FinishedLoading()
        {
            loadingDone = true;
        }
    }
}