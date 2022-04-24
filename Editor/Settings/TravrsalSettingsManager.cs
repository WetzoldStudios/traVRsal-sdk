using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    public static class TravrsalSettingsManager
    {
        private static Settings _sInstance;

        internal static Settings Instance
        {
            get
            {
                if (_sInstance == null) _sInstance = new Settings(SDKUtil.PACKAGE_NAME);

                return _sInstance;
            }
        }

        public static void Save()
        {
            Instance.Save();
        }

        public static T Get<T>(string key, T fallback = default(T), SettingsScope scope = SettingsScope.Project)
        {
            return Instance.Get(key, scope, fallback);
        }

        public static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            Instance.Set(key, value, scope);
        }

        public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            return Instance.ContainsKey<T>(key, scope);
        }
    }
}