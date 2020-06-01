using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    class TravrsalSetting<T> : UserSetting<T>
    {
        public TravrsalSetting(string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(TravrsalSettingsManager.instance, key, value, scope)
        { }

        TravrsalSetting(Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(settings, key, value, scope) { }
    }
}
