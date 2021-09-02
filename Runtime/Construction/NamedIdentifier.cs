using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class NamedIdentifier : MonoBehaviour
    {
        public enum IdentifierType
        {
            None = 39,
            Pivot = 9,
            InventoryItem = 40,

            GameOverDialog = 5,
            SettingsDialog = 1,
            PlayerDialog = 7,
            MultiplayerDialog = 28,
            AvatarDialog = 41,
            InterstitialDialog = 12,
            IntroductionDialog = 22,
            CreateWorldsDialog = 16,
            FailedEntitlementCheckDialog = 24,

            InfoDialog = 10,
            InfoDialogText = 11,
            InfoDialogMainAction = 26,
            InfoDialogAlternativeAction = 27,

            WorldDetailsDialog = 2,
            WorldDetailsDialogPosition = 8,
            WorldsListRoot = 19,
            WorldPlacement = 6,
            WorldBrowserDialog = 20,
            WorldBrowserDialogPosition = 21,
            WorldStartButton = 35,

            ChaptersDialog = 33,
            ChaptersListRoot = 34,
            ChapterCover = 36,

            FeaturedWorldsRoot = 32,
            FeaturedWorldsListRoot = 0,

            ChallengesDialog = 13,
            ChallengesListRoot = 14,

            GlobalLeaderBoardRoot = 43,
            GlobalLeaderListRoot = 18,
            WorldOverallLeaderListRoot = 29,
            WorldSpeedLeaderListRoot = 30,
            WorldAccuracyLeaderListRoot = 31,

            WorldSettingsRoot = 4,
            NoWorldSettingsHint = 3,
            NoChallengesHint = 15,

            MainMenuRoot = 25,
            MainMenuModalCanvasRoot = 38,

            AvatarName = 42,

            TheaterRoot = 37,
            Object = 23
        }

        public IdentifierType type = IdentifierType.None;
        public string optionalData;

        public override string ToString()
        {
            return $"Named Identifier ({type})";
        }
    }
}