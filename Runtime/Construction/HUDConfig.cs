using System;
using UnityEngine;
using static traVRsal.SDK.DataBinding;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class HUDConfig
    {
        public enum Location
        {
            Hands = 0,
            LowerLeft = 1,
            LowerCenter = 2,
            LowerRight = 3,
            UpperLeft = 4,
            UpperCenter = 5,
            UpperRight = 6
        }

        [Header("Configuration")] public Location location = Location.Hands;
        public Reference text = Reference.TimeElapsed;
        public string textWhenZero;
        public bool hideTextWhenZero;
        public string subText;
        public string subTextWhenZero;
        public bool hideSubTextWhenZero;

        [Header("Runtime")] [NonSerialized] public GameObject dataSource;

        public HUDConfig()
        {
        }

        public HUDConfig(Reference text, string textWhenZero, bool hideTextWhenZero, string subText, string subTextWhenZero, bool hideSubTextWhenZero)
        {
            this.text = text;
            this.textWhenZero = textWhenZero;
            this.hideTextWhenZero = hideTextWhenZero;
            this.subText = subText;
            this.subTextWhenZero = subTextWhenZero;
            this.hideSubTextWhenZero = hideSubTextWhenZero;
        }

        public override string ToString()
        {
            return $"HUD Config '{text}'";
        }
    }
}