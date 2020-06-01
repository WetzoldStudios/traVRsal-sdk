using UnityEngine;
using UnityEditor;

namespace traVRsal.SDK
{
    [InitializeOnLoad]
    class AssetRefresher
    {
        private const float SETTINGS_REFRESH = 30f;

        private static float lastUpdate;
        private static float lastRefresh;
        private static int timeInterval = 0;

        static AssetRefresher()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (Time.realtimeSinceStartup - lastRefresh > SETTINGS_REFRESH)
            {
                lastRefresh = Time.realtimeSinceStartup;
                timeInterval = TravrsalSettingsManager.Get<int>("periodicEditorRefresh");
            }

            if (timeInterval > 0 && Time.realtimeSinceStartup - lastUpdate > timeInterval)
            {
                lastUpdate = Time.realtimeSinceStartup;
                AssetDatabase.Refresh();
            }
        }
    }
}