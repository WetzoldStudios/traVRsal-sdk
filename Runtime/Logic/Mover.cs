using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Mover : MonoBehaviour, IVariableReactor
    {
        public enum Mode
        {
            Manual = 0,
            Variable = 1
        }

        public Mode mode = Mode.Manual;
        public Vector2 distance = new Vector2(1f, 1f);
        public float duration = 4f;
        public Vector3 axis = Vector3.up;

        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;

        public float onDelay;
        public float offDelay;

        private float finalDistance;
        private Vector3 originalPosition;
        private bool changedOnce;

        private IEnumerator Start()
        {
            originalPosition = transform.localPosition;
            finalDistance = Random.Range(distance.x, distance.y);

            if (mode == Mode.Manual)
            {
                if (onDelay > 0) yield return new WaitForSeconds(onDelay);

                transform.DOLocalMove(transform.localPosition + axis * finalDistance, duration).SetLoops(loop ? -1 : 0, loopType).SetEase(easeType);
            }
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            if (mode != Mode.Variable) return;

            if (condition)
            {
                transform.DOLocalMove(originalPosition + axis * finalDistance, duration).SetDelay(onDelay + (changedOnce ? 0f : onDelay));
            }
            else
            {
                transform.DOLocalMove(originalPosition, duration).SetDelay(offDelay + (changedOnce ? 0f : onDelay));
            }

            if (!initialCall && variable.everChanged) changedOnce = true;
        }
    }
}