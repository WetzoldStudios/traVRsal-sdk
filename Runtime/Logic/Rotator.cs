﻿using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Rotator")]
    public class Rotator : MonoBehaviour, IVariableReactor, IWorldStateReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        [Header("Configuration")] public Mode mode = Mode.Manual;
        [Range(0, 5)] public int variableChannel;

        [Tooltip("Degrees per second in manual mode. Total degrees otherwise. Use different values for X and Y to define a range for a random angle.")]
        public Vector2 degrees = new Vector2(10f, 10f);

        [Tooltip("Local axis around which the object should be rotated, e.g. (0,1,0) for Y.")]
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Incremental;

        [Header("Timings")] public Vector2 initialDelay;
        public Vector2 duration = new Vector2(1f, 1f);
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
        private bool initStateDone;
        private bool initDone;
        private bool loadingDone;
        private bool continueAfterPause;
        private Tween curTween;

        private void Start()
        {
            if (!initStateDone) InitState();
        }

        private void InitState()
        {
            originalRotationQ = transform.localRotation;
            originalRotation = originalRotationQ.eulerAngles;

            finalDegrees = Random.Range(degrees.x, degrees.y);
            finalInitialDelay = Random.Range(initialDelay.x, initialDelay.y);
            finalDuration = Random.Range(duration.x, duration.y);
            finalOnDelay = Random.Range(onDelay.x, onDelay.y);
            finalOffDelay = Random.Range(offDelay.x, offDelay.y);

            initStateDone = true;
        }

        private void OnEnable()
        {
            if (!loadingDone) return;
            if (initDone)
            {
                if (curTween != null && (continueAfterPause || !curTween.IsComplete())) curTween.Play();
                continueAfterPause = false;
                return;
            }

            // needed for support of initialDelay, since any WaitForSeconds will be interrupted during loading when GO becomes inactive
            startTime = Mathf.Max(Time.time + finalInitialDelay, Single.Epsilon);
        }

        private void OnDisable()
        {
            if (curTween == null) return;

            if (curTween.IsPlaying())
            {
                curTween.Pause();
                continueAfterPause = true;
            }
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
            if (!initStateDone) InitState();
            initDone = true;

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

        public int GetVariableChannel()
        {
            return variableChannel;
        }

        public void ZoneChange(Zone zone, bool isCurrent)
        {
        }

        public void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false)
        {
            loadingDone = true;
            if (instantEnablement) OnEnable();
        }
    }
}