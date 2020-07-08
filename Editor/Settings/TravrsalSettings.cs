using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    class TravrsalSettings : EditorWindow
    {
        [UserSetting("General Settings", "Path to Tiled.exe")]
        static TravrsalSetting<string> tiledPath = new TravrsalSetting<string>("tiledPath", "C:\\Program Files\\Tiled\\tiled.exe", SettingsScope.Project);

        [UserSetting("General Settings", "Periodic Editor Refresh")]
        static TravrsalSetting<int> periodicEditorRefresh = new TravrsalSetting<int>("periodicEditorRefresh", 0, SettingsScope.Project);
    }
}
