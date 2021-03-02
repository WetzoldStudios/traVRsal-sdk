using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Scaler : MonoBehaviour, IVariableReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        public Mode mode = Mode.Manual;
        public Vector2 size = new Vector2(0f, 0f);
        public float duration = 2f;
        public Vector3 axis = Vector3.one;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        public float onDelay;
        public float offDelay;

        private float finalSize;
        private Vector3 originalScale;
        private bool changedOnce;

        private IEnumerator Start()
        {
            originalScale = transform.localScale;
            finalSize = Random.Range(size.x, size.y);

            if (mode == Mode.Manual)
            {
                if (onDelay > 0) yield return new WaitForSeconds(onDelay);
                transform.DOScale(axis * finalSize, duration).SetLoops(loop ? -1 : 0, loopType).SetEase(easeType);
            }
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                transform.DOScale(originalScale + axis * finalSize, duration).SetDelay(onDelay + (changedOnce ? 0f : onDelay));
            }
            else
            {
                transform.DOScale(originalScale, duration).SetDelay(offDelay + (changedOnce ? 0f : onDelay));
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }
    }
}