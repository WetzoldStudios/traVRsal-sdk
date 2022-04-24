using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    static class TravrsalSettingsProvider
    {
        private const string preferencesPath = "Project/traVRsal SDK";

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            // define project scope instead of user scope so that the editor can easily access it
            // assumption is that users don't create many independent unity projects anyway so that should be ok
            UserSettingsProvider provider = new UserSettingsProvider(preferencesPath,
                TravrsalSettingsManager.Instance,
                new[] { typeof(TravrsalSettingsProvider).Assembly }, SettingsScope.Project);

            return provider;
        }
    }
}