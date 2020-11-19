using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    public class Pulser : MonoBehaviour
    {
        public LoopType loopType = LoopType.Restart;
        public float duration = 3f;
        public float startScale = 1.5f;
        public float endScale = 0.15f;

        private void Start()
        {
            transform.localScale = Vector3.one * startScale;
            transform.DOScale(endScale, duration).SetLoops(999, loopType);
        }
    }
}