using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Item Tooltip")]
    public class ItemTooltip : MonoBehaviour
    {
        public float duration = 4f;

        private Transform actor;
        private Vector3 originalScale;

        private void Start()
        {
            actor = transform.GetChild(0);
            originalScale = actor.localScale;
            actor.localScale = Vector3.zero;
            actor.gameObject.SetActive(false);
        }

        public void Trigger()
        {
            if (actor == null) Start();
            actor.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(actor.DOScale(originalScale, duration / 8f));
            seq.Append(actor.DOScale(0, duration / 8f).SetDelay(duration / 8f * 6f));
            seq.OnComplete(() => actor.gameObject.SetActive(false));
        }
    }
}