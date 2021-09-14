using System.Collections;
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

        private IVariableAction context;
        private bool initDone;

        private void Init()
        {
            initDone = true;
            IVariableAction[] contexts = GetComponentsInParent<IVariableAction>(true);
            if (contexts.Length > 0)
            {
                context = contexts[0];
            }
            else
            {
                EDebug.LogError($"Could not find context on {gameObject}");
            }
        }

        public void ReachActionMin()
        {
            if (!initDone) Init();
            if (minSound != null && minSound.clip != null) minSound.Play();

            context.ReachActionMin(variableChannel);
        }

        public void ReachActionMax()
        {
            if (!initDone) Init();
            if (maxSound != null && maxSound.clip != null) maxSound.Play();

            context.ReachActionMax(variableChannel);
        }

        public void ToggleAction()
        {
            if (!initDone) Init();
            context.ToggleAction(variableChannel);
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (!initDone) Init();
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
                        context.ToggleAction(variableReactionChannel);
                        break;

                    case BooleanAction.True:
                        context.ReachActionMax(variableReactionChannel);
                        break;

                    case BooleanAction.False:
                        context.ReachActionMin(variableReactionChannel);
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