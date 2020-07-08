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
            LevelDetailsDialog = 2,
            LevelsRoot = 0,
            NoLevelSettingsHint = 3,
            LevelSettingsRoot = 4
        }

        public IdentifierType type = IdentifierType.LevelsRoot;

        public override string ToString()
        {
            return $"Named identifier ({type})";
        }
    }
}