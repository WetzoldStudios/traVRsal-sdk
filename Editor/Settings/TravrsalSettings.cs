using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    public class TravrsalSettings : EditorWindow
    {
        [UserSetting("General Settings", "Path to Tiled.exe")]
        static TravrsalSetting<string> tiledPath = new TravrsalSetting<string>("tiledPath", SDKUtil.TILED_PATH_DEFAULT, SettingsScope.Project);
    }
}
