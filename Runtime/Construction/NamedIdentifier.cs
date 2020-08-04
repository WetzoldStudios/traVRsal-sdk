using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class NamedIdentifier : MonoBehaviour
    {
        public enum IdentifierType
        {
            GameOverDialog = 5,
            SettingsDialog = 1,
            WorldDetailsDialog = 2,
            WorldsRoot = 0,
            NoWorldSettingsHint = 3,
            WorldSettingsRoot = 4
        }

        public IdentifierType type = IdentifierType.WorldsRoot;

        public override string ToString()
        {
            return $"Named identifier ({type})";
        }
    }
}