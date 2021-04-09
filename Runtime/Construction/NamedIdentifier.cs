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
            MultiplayerDialog = 28,
            InfoDialog = 10,
            InfoDialogText = 11,
            InfoDialogMainAction = 26,
            InfoDialogAlternativeAction = 27,
            WorldDetailsDialog = 2,
            WorldDetailsDialogPosition = 8,
            WorldsListRoot = 19,
            FeaturedWorldsRoot = 32,
            FeaturedWorldsListRoot = 0,
            WorldPlacement = 6,
            WorldBrowserDialog = 20,
            WorldBrowserDialogPosition = 21,
            ChallengesDialog = 13,
            ChallengesListRoot = 14,
            GlobalLeaderListRoot = 18,
            WorldOverallLeaderListRoot = 29,
            WorldSpeedLeaderListRoot = 30,
            WorldAccuracyLeaderListRoot = 31,
            NoWorldSettingsHint = 3,
            NoChallengesHint = 15,
            WorldSettingsRoot = 4,
            InterstitialDialog = 12,
            IntroductionDialog = 22,
            CreateWorldsDialog = 16,
            FailedEntitlementCheckDialog = 24,
            MainMenuRoot = 25,
            Pivot = 9,
            Object = 23
        }

        public IdentifierType type = IdentifierType.FeaturedWorldsListRoot;
        public string optionalData;

        public override string ToString()
        {
            return $"Named Identifier ({type})";
        }
    }
}