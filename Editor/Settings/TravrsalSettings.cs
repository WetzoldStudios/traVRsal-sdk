using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    public class TravrsalSettings : EditorWindow
    {
        [UserSetting("General Settings", "Path to Tiled.exe")]
        static TravrsalSetting<string> tiledPath = new TravrsalSetting<string>("tiledPath", SDKUtil.TILED_PATH_DEFAULT, SettingsScope.Project);

        [UserSetting("General Settings", "Periodic Editor Refresh")]
        static TravrsalSetting<int> periodicEditorRefresh = new TravrsalSetting<int>("periodicEditorRefresh", SDKUtil.PERIODIC_REFRESH_DEFAULT, SettingsScope.Project);
    }
}
