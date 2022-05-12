using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Data Binding")]
    public class DataBinding : ExecutorConfig
    {
        public const int REQUIRE_SOURCE = 1000;
        public const int ASYNC_RESULT = 2000;
        public const int PERIODIC = 3000;

        public enum Reference
        {
            OriginalValue = 0,
            AppVersion = 1,

            UnitLong = 114,
            UnitShort = 115,

            DoOpenWorldDetailsDialog = 18,
            DoCloseWorldDetailsDialog = 13,
            DoCloseChaptersDialog = 156,
            DoOpenSettingsDialog = 20,
            DoCloseSettingsDialog = 21,
            DoOpenSupportDialog = 196,
            DoCloseSupportDialog = 197,
            DoOpenPlayerDialog = 58,
            DoClosePlayerDialog = 59,
            DoCloseInfoDialog = 69,
            DoAlternativeInfoDialogAction = 132,
            DoOpenChallengesDialog = 71,
            DoCloseChallengesDialog = 73,
            DoOpenCreateWorldsDialog = 122,
            DoCloseCreateWorldsDialog = 93,
            DoOpenWorldBrowserDialog = 187,
            DoCloseWorldBrowserDialog = 121,
            DoOpenMultiplayerDialog = 134,
            DoCloseMultiplayerDialog = 135,
            DoOpenAvatarDialog = 160,
            DoCloseAvatarDialog = 161,
            DoCloseIntroductionDialog = 186,
            DoBuySupporterPack = 198,
            DoQuit = 126,
            DoPause = 15,
            DoUnpause = 16,
            DoReloadMain = 185,
            DoReturnToMain = 17,
            DoReturnToMainInterstitial = 70,
            DoPostChallenge = 72,
            DoStartIntro = 184,
            DoStartWorld = 14,
            DoStartWorldDownload = 67,
            DoStartWorldPurchase = 191,
            DoStartRandomChallenge = 109,
            DoReplayWorld = 148,
            DoPlayWorldTrailer = 173,
            DoSetDebugMode = 7,
            DoSetMetricSystem = 116,
            DoSetHaptics = 125,
            DoSetHQShaders = 158,
            DoSetChallengeGhosts = 165,
            DoSendReport = 192,
            DoSendDelayedReport = 193,
            DoLogOut = 63,
            DoClearCache = 68,
            DoResetSettings = 111,
            DoResetHints = 151,
            DoChallengeFilterMine = 94,
            DoChallengeFilterOthers = 95,
            DoChallengeFilterPlayspace = 100,
            DoChallengeFilterNotBeaten = 97,
            DoChallengeFilterBeaten = 96,
            DoChallengeFilterBest = 98,
            DoOpenSDKWebsite = 129,
            DoLeaveParty = 141,
            DoSwitchToControllers = 175,
            DoSwitchToWalking = 176,
            DoIncStickModeX = 180,
            DoDecStickModeX = 181,
            DoIncStickModeY = 182,
            DoDecStickModeY = 183,

            EnterLogin = 64,
            EnterNickname = 62,
            EnterNicknameWithTerms = 99,
            EnterPassword = 65,
            EnterRPMCode = 162,
            EnterRoomCode = 136,

            WorldCategory = 10,
            WorldCover = ASYNC_RESULT + 3,
            WorldName = 2,
            WorldShortDescription = 11,
            WorldLongDescription = 12,
            WorldPrice = 190,
            WorldTimer = PERIODIC + 3,
            WorldUpdate = 56,
            WorldUpdateRelative = 92,
            WorldSize = 55,
            WorldOwner = 66,
            WorldCreators = 123,
            WorldLives = 166,

            ZoneTime = PERIODIC + 5,
            ZoneTimer = PERIODIC + 4,

            PlayerDistance = 9,
            PlayerDistanceNoUnit = 119,
            PlayerName = 3,
            PlayerNameWithHints = 117,
            PlayerLogin = 57,
            PlayerAvatarUrl = 163,
            PlayerImage = ASYNC_RESULT + 4,
            PlayerLives = 167,

            SupportPackageCount = 199,

            MPPlayerCount = 137,
            MPPlayerNames = 143,
            MPPlayerNamesWithDetails = 146,
            MPPlayerNamesWithStatus = 169,
            MPRoomName = 140,
            MPHostName = 142,
            MPWorldName = 145,
            MPMaxPlayArea = 144,

            Item = 120,
            Points = 124,
            AllTargets = 130,
            RemainingTargets = 131,
            FirstRemainingTarget = 133,
            Health = 153,

            TimeElapsed = PERIODIC + 1,
            SlowMotionCooldownRemaining = PERIODIC + 2,
            DistanceWalked = PERIODIC + 6,

            SettingPadding = 6,
            SettingTileCount = 5,
            SettingRenderScale = 164,
            SettingTileSizeHint = 8,
            SettingLocomotionMethod = 174,
            SettingStickModeX = 178,
            SettingStickModeY = 179,

            StatAccuracy = 23,
            StatDeaths = 25,
            StatDistanceWalked = 26,
            StatDistanceWalkedNoUnit = 118,
            StatPlayerDamage = 30,
            StatPoints = 27,
            StatRuns = 150,
            StatShotsFired = 29,
            StatShotsHit = 28,
            StatTargets = 32,
            StatTargetsDestroyed = 31,
            StatTargetsDestroyedRatio = 24,
            StatTimeOnCriticalPath = 22,

            ChallengePlayer = 110,
            ChallengeStatAccuracy = 81,
            ChallengeStatDeaths = 82,
            ChallengeStatDistanceWalked = 83,
            ChallengeStatPlayerDamage = 84,
            ChallengeStatPoints = 85,
            ChallengeStatShotsFired = 86,
            ChallengeStatShotsHit = 87,
            ChallengeStatTargets = 88,
            ChallengeStatTargetsDestroyed = 89,
            ChallengeStatTargetsDestroyedRatio = 90,
            ChallengeStatTimeOnCriticalPath = 91,

            ChallengesPosted = 101,
            ChallengesPostedBeaten = 102,
            ChallengesPostedBeatenPercentage = 106,
            ChallengesPostedTries = 108,
            ChallengesBeaten = 103,
            ChallengesBeatenBest = 104,
            ChallengesRemaining = 105,
            ChallengesBeatenRemainingPercentage = 107,

            ShowStatAccuracy = 34,
            ShowStatDeaths = 38,
            ShowStatDistanceWalked = 39,
            ShowStatPoints = 40,
            ShowStatTargets = 37,
            ShowStatTimeOnCriticalPath = 33,
            ShowPostChallenge = 74,

            ShowIfChallengeMode = 79,
            ShowIfChallengeWonNormal = 77,
            ShowIfChallengeWonBest = 78,
            ShowIfChallengeWon = 80,

            ShowIfProduction = 155,
            ShowIfBeta = 112,
            ShowIfAlpha = 154,
            ShowIfOffline = 113,
            ShowIfOnlineAndNotInMP = 168,
            ShowIfMPPossible = 147,
            ShowIfMPHost = 138,
            ShowIfMPParticipant = 171,
            ShowIfInMPSession = 139,
            ShowPlayerLogin = 60,
            ShowPlayerLoggedIn = 61,
            ShowCustomPlayerImage = 127,
            ShowIfPlatformPlayer = 128,
            ShowIfCommunityWorld = 152,
            ShowIfRecordingAvailable = 170,
            ShowIfHaptics = 157,
            ShowIfControllerMovement = 177,
            ShowIfInMainMenu = 188,

            ShowWorldDownload = ASYNC_RESULT + 1,
            ShowWorldStart = ASYNC_RESULT + 2,
            ShowWorldPurchase = 189,
            ShowWorldPurchaseChapterAlternative = 195,
            ShowWorldChallenges = 76,
            ShowWorldStatistics = 75,
            ShowWorldLeaderboards = 149,
            ShowWorldTrailer = 172,
            ShowWorldTestimonials = 200,

            ObjectHealth = REQUIRE_SOURCE + 50,

            ImageOrigin = REQUIRE_SOURCE + 8,
            ImageOriginLogo = REQUIRE_SOURCE + 13,
            ImageName = REQUIRE_SOURCE + 1,
            ImageDescription = REQUIRE_SOURCE + 2,
            ImageDescriptionOrName = REQUIRE_SOURCE + 9,
            ImageDate = REQUIRE_SOURCE + 3,
            ImageAuthor = REQUIRE_SOURCE + 4,
            ImageAuthorLink = REQUIRE_SOURCE + 5,
            ImageLink = REQUIRE_SOURCE + 6,
            ImageAudio = REQUIRE_SOURCE + 10,
            ImageSpeech = REQUIRE_SOURCE + 11,
            ImageSpeechOrDescription = REQUIRE_SOURCE + 12,
            ImageRatingsCount = REQUIRE_SOURCE + 7
        }

        [Header("Configuration")] [Tooltip("Optional source for the data (e.g. image assignment), otherwise using globally available data")]
        public Component sourceComponent;

        public Component targetComponent;
        public Reference reference;

        [Header("Boolean-Bindings")] [Tooltip("Indicator if boolean result should be checked for false instead of true")]
        public bool invert;

        [Header("Text-Bindings")] [Tooltip("Maximum number of characters to be returned (... added if longer)")]
        public int maxLength;

        [Tooltip("Maximum number of lines to be returned (... added if longer)")]
        public int maxLines;

        [Tooltip("Text to be shown if value is empty")]
        public string placeHolder;

        [Header("Advanced")] [Tooltip("Indicator if result should be calculated only once")]
        public bool oneTimeOnly;

        [HideInInspector] public IDataSource dataSource;
        [HideInInspector] public bool isTriggered;

        public void Trigger()
        {
            isTriggered = true;
        }
    }
}