using UnityEngine;

namespace traVRsal.SDK
{
    public class DataBinding : ExecutorConfig
    {
        public enum Reference
        {
            OriginalValue = 0,
            AppVersion = 1,
            DebugMode = 7,
            DoOpenLevelDetailsDialog = 18,
            DoCloseLevelDetailsDialog = 13,
            DoOpenSettingsDialog = 20,
            DoCloseSettingsDialog = 21,
            DoPause = 15,
            DoReturnToMain = 17,
            DoStartLevel = 14,
            DoUnpause = 16,
            LevelCategory = 10,
            LevelCover = 19,
            LevelLongDescription = 12,
            LevelName = 2,
            LevelShortDescription = 11,
            LevelTime = 4,
            Padding = 6,
            PlayerDistance = 9,
            PlayerName = 3,
            TileCount = 5,
            TileSizeHint = 8
        }

        public Behaviour targetComponent;
        public Reference reference;
        public bool oneTimeOnly = false;
    }
}