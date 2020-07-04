using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class NamedIdentifier : MonoBehaviour
    {
        public enum IdentifierType
        {
            LevelCoverImage = 1,
            LevelDetailsDialog = 2,
            LevelsRoot = 0,
            NoSettingsHint = 3,
            SettingsRoot = 4
        }

        public IdentifierType type = IdentifierType.LevelsRoot;

        public override string ToString()
        {
            return $"Named identifier ({type})";
        }
    }
}