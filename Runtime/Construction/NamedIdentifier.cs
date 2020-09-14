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
            InfoDialog = 10,
            InfoDialogText = 11,
            WorldDetailsDialog = 2,
            WorldDetailsDialogPosition = 8,
            WorldsListRoot = 0,
            WorldPlacement = 6,
            NoWorldSettingsHint = 3,
            WorldSettingsRoot = 4,
            Pivot = 9
        }

        public IdentifierType type = IdentifierType.WorldsListRoot;
        public string optionalData;

        public override string ToString()
        {
            return $"Named identifier ({type})";
        }
    }
}