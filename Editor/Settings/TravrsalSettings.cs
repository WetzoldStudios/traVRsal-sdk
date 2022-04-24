using UnityEditor;
using UnityEditor.SettingsManagement;

namespace traVRsal.SDK
{
    public class TravrsalSettings : EditorWindow
    {
        [UserSetting("General Settings", "Creator Key")]
        static TravrsalSetting<string> apiKey = new TravrsalSetting<string>("apiKey", null);

        [UserSetting("General Settings", "Path to Tiled executable")]
        static TravrsalSetting<string> tiledPath = new TravrsalSetting<string>("tiledPath", SDKUtil.TILED_PATH_DEFAULT);

        [UserSetting("General Settings", "Replica Login")]
        static TravrsalSetting<string> replicaLogin = new TravrsalSetting<string>("replicaLogin", null);

        [UserSetting("General Settings", "Replica Password")]
        static TravrsalSetting<string> replicaPassword = new TravrsalSetting<string>("replicaPassword", null);
    }
}