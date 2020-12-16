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
            WorldsListRoot = 19,
            FeaturedWorldsListRoot = 0,
            WorldPlacement = 6,
            WorldBrowserDialog = 20,
            WorldBrowserDialogPosition = 21,
            ChallengesDialog = 13,
            ChallengesListRoot = 14,
            HighscoreListRoot = 18,
            NoWorldSettingsHint = 3,
            NoChallengesHint = 15,
            WorldSettingsRoot = 4,
            InterstitialDialog = 12,
            IntroductionDialog = 22,
            CreateWorldsDialog = 16,
            Pivot = 9
        }

        public IdentifierType type = IdentifierType.FeaturedWorldsListRoot;
        public string optionalData;

        public override string ToString()
        {
            return $"Named Identifier ({type})";
        }
    }
}