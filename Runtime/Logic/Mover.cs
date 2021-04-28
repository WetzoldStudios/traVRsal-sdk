using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    public class Mover : MonoBehaviour, IVariableReactor, IWorldStateReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        public enum ValueScale
        {
            Meters = 0,
            Tiles = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;
        public ValueScale valueScale = ValueScale.Meters;

        [Tooltip("Distance the object should travel. Use different values for X and Y to define a range for a random distance.")]
        public Vector2 distance = new Vector2(1f, 1f);

        [Tooltip("Direction the object should travel, e.g. (0,1,0) for up.")]
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        [Header("Timings")] public Vector2 initialDelay;
        public Vector2 duration = new Vector2(4f, 4f);
        public Vector2 onDelay;
        public Vector2 offDelay;

        [Header("Audio")] [Tooltip("Sound to play when movement is started.")]
        public AudioSource audio;

        private float finalDistance;
        private float finalInitialDelay;
        private float finalDuration;
        private float finalOnDelay;
        private float finalOffDelay;

        private Vector3 originalPosition;
        private bool changedOnce;
        private float startTime;
        private bool loadingDone;
        private bool initDone;
        private Tween curTween;

        private void Start()
        {
            originalPosition = transform.localPosition;
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
            if (finalDistance == 0) return;

            Sequence s = DOTween.Sequence();
            s.PrependInterval(finalOnDelay);
            s.AppendCallback(PlayAudio); // OnPlay is only called once in a sequence
            s.Append(transform.DOLocalMove(transform.localPosition + axis * finalDistance, finalDuration).SetEase(easeType));
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
                curTween = transform.DOLocalMove(originalPosition + axis * finalDistance, finalDuration)
                    .SetDelay(finalOnDelay + (changedOnce ? 0f : finalInitialDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }
            else
            {
                curTween = transform.DOLocalMove(originalPosition, finalDuration)
                    .SetDelay(finalOffDelay + (changedOnce ? 0f : finalInitialDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }

        public void FinishedLoading(Vector3 tileSizes)
        {
            finalDistance = Random.Range(distance.x, distance.y);

            // multiply with tile size to fit into world grid
            if (valueScale == ValueScale.Tiles)
            {
                finalDistance *= axis.y != 0 ? tileSizes.y : tileSizes.x;
            }

            finalInitialDelay = Random.Range(initialDelay.x, initialDelay.y);
            finalDuration = Random.Range(duration.x, duration.y);
            finalOnDelay = Random.Range(onDelay.x, onDelay.y);
            finalOffDelay = Random.Range(offDelay.x, offDelay.y);

            loadingDone = true;
        }
    }
}