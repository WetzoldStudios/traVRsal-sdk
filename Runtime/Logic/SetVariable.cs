﻿using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Set Variable")]
    public class SetVariable : MonoBehaviour, IVariableReactor
    {
        public enum Mode
        {
            ActiveOnly = 0,
            ActiveAndReactive = 1
        }

        public enum BooleanAction
        {
            Toggle = 0,
            True = 1,
            False = 2
        }

        public Mode mode;

        [Header("Active Configuration")] [Range(0, 5)]
        public int variableChannel;

        public AudioSource maxSound;
        public AudioSource minSound;

        [Header("Reactive Configuration")] [Range(0, 5)]
        public int variableReactionChannel;

        public float delay;

        [Header("If Boolean")] public BooleanAction booleanAction;

        private IVariableAction _context;
        private bool _initDone;

        private void Init()
        {
            _initDone = true;
            IVariableAction[] contexts = GetComponentsInParent<IVariableAction>(true);
            if (contexts.Length > 0)
            {
                _context = contexts[0];
            }
            else
            {
                EDebug.LogError($"Could not find context on {gameObject}");
            }
        }

        public void ReachActionMin()
        {
            if (!_initDone) Init();
            if (minSound != null && minSound.clip != null) minSound.Play();

            _context.ReachActionMin(variableChannel);
        }

        public void ReachActionMin(string key)
        {
            if (!_initDone) Init();
            if (minSound != null && minSound.clip != null) minSound.Play();

            _context.ReachActionMin(key);
        }

        public void ReachActionMax()
        {
            if (!_initDone) Init();
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            _context.ReachActionMax(variableChannel);
        }

        public void ReachActionMax(string key)
        {
            if (!_initDone) Init();
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            _context.ReachActionMax(key);
        }

        public void ToggleAction()
        {
            if (!_initDone) Init();
            _context.ToggleAction(variableChannel);
        }

        public void ToggleAction(string key)
        {
            if (!_initDone) Init();
            _context.ToggleAction(key);
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (!_initDone) Init();
            if (mode != Mode.ActiveAndReactive) return;
            if (initialCall) return;

            StartCoroutine(DoVariableChanged(variable, condition, initialCall));
        }

        private IEnumerator DoVariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);

            if (variable.value is bool)
            {
                switch (booleanAction)
                {
                    case BooleanAction.Toggle:
                        _context.ToggleAction(variableReactionChannel);
                        break;

                    case BooleanAction.True:
                        _context.ReachActionMax(variableReactionChannel);
                        break;

                    case BooleanAction.False:
                        _context.ReachActionMin(variableReactionChannel);
                        break;
                }
            }
        }

        public int GetVariableChannel()
        {
            return variableChannel;
        }
    }
}