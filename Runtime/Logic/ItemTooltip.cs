using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Item Tooltip")]
    public class ItemTooltip : MonoBehaviour
    {
        public float duration = 4f;

        private Transform _actor;
        private Vector3 _originalScale;

        private void Start()
        {
            _actor = transform.GetChild(0);
            _originalScale = _actor.localScale;
            _actor.localScale = Vector3.zero;
            _actor.gameObject.SetActive(false);
        }

        public void Trigger()
        {
            if (_actor == null) Start();
            _actor.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(_actor.DOScale(_originalScale, duration / 8f));
            seq.Append(_actor.DOScale(0, duration / 8f).SetDelay(duration / 8f * 6f));
            seq.OnComplete(() => _actor.gameObject.SetActive(false));
        }
    }
}