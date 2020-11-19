using UnityEngine;
using static traVRsal.SDK.DataBinding;

namespace traVRsal.SDK
{
    public class HUDInfo : MonoBehaviour
    {
        public Reference text;
        public string textWhenZero;
        public string subText;
        public string subTextWhenZero;
        public bool hideSubTextWhenZero;

        private void Start()
        {
        }
    }
}