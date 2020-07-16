using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    public static class TravrsalSettingsManager
    {
        private static Settings sInstance;

        internal static Settings instance
        {
            get
            {
                if (sInstance == null) sInstance = new Settings(SDKUtil.PACKAGE_NAME);

                return sInstance;
            }
        }

        public static void Save()
        {
            instance.Save();
        }

        public static T Get<T>(string key, T fallback = default(T), SettingsScope scope = SettingsScope.Project)
        {
            return instance.Get<T>(key, scope, fallback);
        }

        public static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            instance.Set<T>(key, value, scope);
        }

        public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            return instance.ContainsKey<T>(key, scope);
        }
    }
}