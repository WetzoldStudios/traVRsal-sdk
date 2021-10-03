using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Scaler")]
    public class Scaler : MonoBehaviour, IVariableReactor, IWorldStateReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;
        [Range(0, 5)] public int variableChannel;

        [Tooltip("Scale of the object, e.g. (1.2,1.2) for 20% bigger. Use different values for X and Y to define a range for a random scale.")]
        public Vector2 size = new Vector2(0f, 0f);

        [Tooltip("Axis on which the object should be scaled, e.g. (0,1,0) for Y only.")]
        public Vector3 axis = Vector3.one;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        [Header("Timings")] public Vector2 initialDelay;
        public Vector2 duration = new Vector2(1f, 1f);
        public Vector2 onDelay;
        public Vector2 offDelay;

        [Header("Audio")] [Tooltip("Sound to play when scaling is started.")]
        public AudioSource audio;

        private float _finalSize;
        private float _finalInitialDelay;
        private float _finalDuration;
        private float _finalOnDelay;
        private float _finalOffDelay;

        private Vector3 _originalScale;
        private bool _changedOnce;
        private float _startTime;
        private bool _initStateDone;
        private bool _initDone;
        private bool _loadingDone;
        private bool _continueAfterPause;
        private Tween _curTween;

        private void Start()
        {
            if (!_initStateDone) InitState();
        }

        private void InitState()
        {
            _originalScale = transform.localScale;

            _finalSize = Random.Range(size.x, size.y);
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
            _startTime = Mathf.Max(Time.time + _finalInitialDelay, Single.Epsilon);
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
            Sequence s = DOTween.Sequence();
            s.PrependInterval(_finalOnDelay);
            s.AppendCallback(PlayAudio); // OnPlay is only called once in a sequence
            s.Append(transform.DOScale(axis * _finalSize, _finalDuration).SetLoops(loop ? -1 : 0, loopType).SetEase(easeType));
            s.AppendInterval(_finalOffDelay);
            s.SetLoops(loop ? -1 : 0, loopType);

            _curTween = s;
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

            if (condition)
            {
                _curTween = transform.DOScale(_originalScale + axis * _finalSize, _finalDuration).SetDelay(_finalOnDelay + (_changedOnce ? 0f : _finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }
            else
            {
                _curTween = transform.DOScale(_originalScale, _finalDuration).SetDelay(_finalOffDelay + (_changedOnce ? 0f : _finalOnDelay)).SetEase(easeType).OnPlay(PlayAudio);
            }

            if (!initialCall && variable.everChanged) _changedOnce = true;
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            _loadingDone = true;
            if (instantEnablement) OnEnable();
        }
    }
}