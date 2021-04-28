using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Scaler : MonoBehaviour, IVariableReactor, IWorldStateReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;

        [Tooltip("Scale of the object, e.g. (1.2,1.2) for 20% bigger. Use different values for X and Y to define a range for a random scale.")]
        public Vector2 size = new Vector2(0f, 0f);

        [Tooltip("Axis on which the object should be scaled, e.g. (0,1,0) for Y only.")]
        public Vector3 axis = Vector3.one;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        [Header("Timings")] public Vector2 initialDelay;
        public Vector2 duration = new Vector2(2f, 2f);
        public Vector2 onDelay;
        public Vector2 offDelay;

        [Header("Audio")] [Tooltip("Sound to play when scaling is started.")]
        public AudioSource audio;

        private float finalSize;
        private float finalInitialDelay;
        private float finalDuration;
        private float finalOnDelay;
        private float finalOffDelay;

        private Vector3 originalScale;
        private bool changedOnce;
        private float startTime;
        private bool initDone;
        private bool loadingDone;
        private Tween curTween;

        private void Start()
        {
            originalScale = transform.localScale;

            finalSize = Random.Range(size.x, size.y);
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
            if (finalSize == 0) return;

            Sequence s = DOTween.Sequence();
            s.PrependInterval(finalOnDelay);
            s.AppendCallback(PlayAudio); // OnPlay is only called once in a sequence
            s.Append(transform.DOScale(axis * finalSize, finalDuration).SetLoops(loop ? -1 : 0, loopType).SetEase(easeType));
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
                curTween = transform.DOScale(originalScale + axis * finalSize, finalDuration).SetDelay(finalOnDelay + (changedOnce ? 0f : finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }
            else
            {
                curTween = transform.DOScale(originalScale, finalDuration).SetDelay(finalOffDelay + (changedOnce ? 0f : finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }

        public void FinishedLoading(Vector3 tileSizes)
        {
            loadingDone = true;
        }
    }
}