using DG.Tweening;
using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Blend Shape Trigger")]
    public class BlendShapeTrigger : MonoBehaviour, IVariableReactor
    {
        [Header("Static References")] [Tooltip("Skinned mesh renderer containing the blend shape. If empty will use first one found in hierarchy.")]
        public SkinnedMeshRenderer skinnedRenderer;

        [Header("Configuration")] public float duration = 2f;
        public float minValue;
        public float maxValue = 100f;
        public int shapeIndex;
        public bool invert;

        private Tween curTween;

        private void Start()
        {
            if (skinnedRenderer == null) skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void VariableChanged(Variable variable, bool condition, bool initialCall = false)
        {
            float targetValue = condition ? (invert ? minValue : maxValue) : (invert ? maxValue : minValue);

            curTween?.Kill();
            curTween = DOTween.To(() => skinnedRenderer.GetBlendShapeWeight(shapeIndex), x => skinnedRenderer.SetBlendShapeWeight(shapeIndex, x), targetValue, duration);
        }
    }
}