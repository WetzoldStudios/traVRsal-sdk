using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Mover")]
    public class Mover : MonoBehaviour, IWorldStateReactor, IVariableReactor
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
        [Range(0, 5)] public int variableChannel;
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

        [Header("Events")] public UnityEvent onReachedDestination;

        private float _finalDistance;
        private float _finalInitialDelay;
        private float _finalDuration;
        private float _finalOnDelay;
        private float _finalOffDelay;

        private Vector3 _originalPosition;
        private bool _changedOnce;
        private float _startTime;
        private bool _loadingDone;
        private bool _initStateDone;
        private bool _initDone;
        private bool _continueAfterPause;
        private Tween _curTween;

        private void Start()
        {
            if (!_initStateDone) InitState();
        }

        private void InitState()
        {
            _originalPosition = transform.localPosition;

            _finalDistance = Random.Range(distance.x, distance.y);
            _finalInitialDelay = Random.Range(initialDelay.x, initialDelay.y);
            _finalDuration = Random.Range(duration.x, duration.y);
            _finalOnDelay = Random.Range(onDelay.x, onDelay.y);
            _finalOffDelay = Random.Range(offDelay.x, offDelay.y);

            _initStateDone = true;
        }

        private void OnEnable()
        {
            if (!_loadingDone) return;
            if (_initDone)
            {
                if (_curTween != null && (_continueAfterPause || !_curTween.IsComplete())) _curTween.Play();
                _continueAfterPause = false;
                return;
            }

            // needed for support of initialDelay, since any WaitForSeconds will be interrupted during loading when GO becomes inactive
            _startTime = Mathf.Max(Time.time + _finalInitialDelay, float.Epsilon);
        }

        private void OnDisable()
        {
            if (_curTween == null) return;

            if (_curTween.IsPlaying())
            {
                _curTween.Pause();
                _continueAfterPause = true;
            }
        }

        private void Update()
        {
            if (_initDone || mode != Mode.Manual) return;
            if (_startTime > 0 && Time.time > _startTime)
            {
                SetupManual();
                _initDone = true;
            }
        }

        private void SetupManual()
        {
            if (_finalDistance == 0) return;

            Sequence s = DOTween.Sequence();
            s.PrependInterval(_finalOnDelay);
            s.AppendCallback(PlayAudio); // OnPlay is only called once in a sequence
            s.Append(transform.DOLocalMove(transform.localPosition + axis * _finalDistance, _finalDuration).SetEase(easeType));
            s.AppendInterval(_finalOffDelay);
            s.AppendCallback(() => onReachedDestination?.Invoke());
            s.SetLoops(loop ? -1 : 0, loopType);

            _curTween = s;
        }

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            _curTween?.Play();
        }

        private void PlayAudio()
        {
            if (audio != null) audio.Play();
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;
            if (!_initStateDone) InitState();
            _initDone = true;

            if (_curTween != null) DOTween.Kill(_curTween);
            if (variable.value is bool)
            {
                if (condition)
                {
                    _curTween = transform.DOLocalMove(_originalPosition + axis * _finalDistance, _finalDuration)
                        .SetDelay(_finalOnDelay + (_changedOnce ? 0f : _finalInitialDelay))
                        .SetEase(easeType).OnPlay(PlayAudio).OnComplete(() => onReachedDestination?.Invoke());
                }
                else
                {
                    _curTween = transform.DOLocalMove(_originalPosition, _finalDuration)
                        .SetDelay(_finalOffDelay + (_changedOnce ? 0f : _finalInitialDelay))
                        .SetEase(easeType).OnPlay(PlayAudio);
                }
            }
            else
            {
                MovePartially(variable.GetNumeric());
            }
            if (!initialCall && variable.everChanged) _changedOnce = true;
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }

        private void MovePartially(float dist)
        {
            Vector3 pos = _originalPosition + (axis * _finalDistance - _originalPosition) * dist;

            if (_curTween != null) DOTween.Kill(_curTween);
            _curTween = transform.DOLocalMove(pos, _finalDuration)
                .SetDelay(_finalOffDelay + (_changedOnce ? 0f : _finalInitialDelay)).SetEase(easeType).OnPlay(PlayAudio);
        }

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            if (!_initStateDone) InitState();

            // multiply with tile size to fit into world grid
            if (valueScale == ValueScale.Tiles)
            {
                _finalDistance *= axis.y != 0 ? tileSizes.y : tileSizes.x;
            }

            _loadingDone = true;
            if (instantEnablement) OnEnable();
        }
    }
}