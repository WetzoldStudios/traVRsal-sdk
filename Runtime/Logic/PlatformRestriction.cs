using System;
using System.Linq;
using UnityEngine;

namespace traVRsal.SDK
{
    [Obsolete]
    public class PlatformRestriction : MonoBehaviour
    {
        public enum Mode
        {
            Disable = 0,
            Destroy = 1
        }

        public RuntimePlatform[] platforms;
        public Mode mode = Mode.Disable;

        private void Awake()
        {
            EDebug.LogWarning("Platform Restriction is outdated. Switch to Environment Restriction.");

            bool valid = platforms.Any(p => p == Application.platform);
            if (valid) return;

            if (mode == Mode.Disable)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}