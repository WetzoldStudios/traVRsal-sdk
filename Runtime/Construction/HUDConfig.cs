using System;
using static traVRsal.SDK.DataBinding;

namespace traVRsal.SDK
{
    [Serializable]
    public class HUDConfig
    {
        public Reference text = Reference.TimeElapsed;
        public string textWhenZero;
        public bool hideTextWhenZero;
        public string subText;
        public string subTextWhenZero;
        public bool hideSubTextWhenZero;
    }
}