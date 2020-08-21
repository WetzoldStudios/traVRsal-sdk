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
            PlayerDialog = 7,
            WorldDetailsDialog = 2,
            WorldsListRoot = 0,
            WorldPlacement = 6,
            NoWorldSettingsHint = 3,
            WorldSettingsRoot = 4
        }

        public IdentifierType type = IdentifierType.WorldsListRoot;
        public string optionalData;

        public override string ToString()
        {
            return $"Named identifier ({type})";
        }
    }
}