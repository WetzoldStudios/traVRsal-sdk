using UnityEngine;

namespace traVRsal.SDK
{
    public class DataBinding : ExecutorConfig
    {
        public const int REQUIRE_SOURCE = 1000;

        public enum Reference
        {
            OriginalValue = 0,
            AppVersion = 1,

            DoOpenWorldDetailsDialog = 18,
            DoCloseWorldDetailsDialog = 13,
            DoOpenSettingsDialog = 20,
            DoCloseSettingsDialog = 21,
            DoPause = 15,
            DoReturnToMain = 17,
            DoStartWorld = 14,
            DoUnpause = 16,

            WorldCategory = 10,
            WorldCover = 19,
            WorldLongDescription = 12,
            WorldName = 2,
            WorldShortDescription = 11,
            WorldTime = 4,

            PlayerDistance = 9,
            PlayerName = 3,

            SettingDebugMode = 7,
            SettingPadding = 6,
            SettingTileCount = 5,
            SettingTileSizeHint = 8,

            StatAccuracy = 23,
            StatDeaths = 25,
            StatDistanceWalked = 26,
            StatPlayerDamage = 30,
            StatPoints = 27,
            StatShotsFired = 29,
            StatShotsHit = 28,
            StatTargets = 32,
            StatTargetsDestroyed = 31,
            StatTargetsDestroyedRatio = 24,
            StatTimeOnCriticalPath = 22,

            ShowStatAccuracy = 34,
            ShowStatDeaths = 38,
            ShowStatDistanceWalked = 39,
            ShowStatPoints = 40,
            ShowStatTargets = 37,
            ShowStatTimeOnCriticalPath = 33,

            ShowDetailStatAccuracy = 44,
            ShowDetailStatDeaths = 45,
            ShowDetailStatDistanceWalked = 46,
            ShowDetailStatPlayerDamage = 47,
            ShowDetailStatPoints = 48,
            ShowDetailStatTargets = 51,
            ShowDetailStatTimeOnCriticalPath = 54,

            ImageName = REQUIRE_SOURCE + 1,
            ImageDescription = REQUIRE_SOURCE + 2,
            ImageDate = REQUIRE_SOURCE +3,
            ImageAuthor = REQUIRE_SOURCE + 4,
            ImageAuthorLink = REQUIRE_SOURCE + 5,
            ImageLink = REQUIRE_SOURCE + 6,
            ImageRatingsCount = REQUIRE_SOURCE + 7
        }

        [Tooltip("Optional source for the data (e.g. image assignment), otherwise using globally available data")]
        public Behaviour sourceComponent;
        public Behaviour targetComponent;
        public Reference reference;
        public bool oneTimeOnly = false;

        [HideInInspector]
        public IDataSource dataSource;
    }
}