using System.Linq;
using UnityEngine;

namespace traVRsal.SDK
{
    public class PlatformRestriction : MonoBehaviour
    {
        public RuntimePlatform[] platforms;

        private void Start()
        {
            bool valid = platforms.Any(p => p == Application.platform);
            if (!valid) gameObject.SetActive(false);
        }
    }
}