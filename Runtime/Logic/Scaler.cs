using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Scaler : MonoBehaviour
    {
        public Vector2 size = new Vector2(0f, 0f);
        public float duration = 2f;
        public Vector3 axis = Vector3.one;
        public Ease easeType = Ease.InOutSine;
        public bool loop = true;
        public LoopType loopType = LoopType.Yoyo;
        public float initialDelay;

        private IEnumerator Start()
        {
            if (initialDelay > 0) yield return new WaitForSeconds(initialDelay);

            float finalSize = Random.Range(size.x, size.y);
            transform.DOScale(axis * finalSize, duration).SetLoops(loop ? -1 : 0, loopType).SetEase(easeType);
        }
    }
}