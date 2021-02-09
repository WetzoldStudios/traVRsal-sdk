using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Mover : MonoBehaviour
    {
        public Vector2 distance = new Vector2(1f, 1f);
        public float duration = 4f;
        public Vector3 axis = Vector3.up;
        public Ease easeType = Ease.InOutSine;
        public LoopType loopType = LoopType.Yoyo;

        private void Start()
        {
            float finalDistance = Random.Range(distance.x, distance.y);
            transform.DOLocalMove(transform.localPosition + axis * finalDistance, duration).SetLoops(-1, loopType).SetEase(easeType);
        }
    }
}