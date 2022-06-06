using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class SetupUI : BasicEditorUI
    {
        private int _worldIdx;
        private string _worldName;
        private bool _showMaintenance;

        [MenuItem("traVRsal/Setup", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<SetupUI>("traVRsal Setup");
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Create your own worlds and amaze other players! A sample world will help you to get started.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            if (CheckTokenGUI())
            {
                if (userWorlds == null || userWorlds.Length == 0)
                {
                    EditorGUILayout.HelpBox("You have not registered any worlds yet on www.traVRsal.com.", MessageType.Info);
                }
                else
                {
                    _worldIdx = EditorGUILayout.Popup("Registered World Keys:", _worldIdx, userWorlds.Select(w => w.key).ToArray());
                    _worldName = userWorlds[_worldIdx].key;
                    if (GUILayout.Button("Create Selected World")) CreateSampleWorld();
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh List")) EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
                if (GUILayout.Button("Register New")) Help.BrowseURL("https://www.traVRsal.com/home");
                GUILayout.EndHorizontal();
            }

            if (Directory.Exists(GetWorldsRoot(false)))
            {
                EditorGUILayout.Space();
                _showMaintenance = EditorGUILayout.Foldout(_showMaintenance, "Maintenance Functions");
                if (_showMaintenance)
                {
                    if (GUILayout.Button("Update/Restore Tiled Data")) RestoreTiled();
                }
            }

            OnGUIDone();
        }

        private string GetWorldPath(bool relative)
        {
            return GetWorldsRoot(relative) + "/" + _worldName;
        }

        private void CreateSampleWorld()
        {
            if (string.IsNullOrEmpty(_worldName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "No world key specified.", "OK");
                return;
            }
            if (!IsValidWorldName(_worldName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "World key is not valid: must be upper and lower case characters, numbers and underscore only.", "OK");
                return;
            }

            if (CreateWorldsRoot())
            {
                RestoreTiled();
                SetupGitIgnore();
            }
            CopySampleWorld();

            EditorUtility.FocusProjectWindow();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetWorldPath(true));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);

            _worldName = "";
        }

        private void SetupGitIgnore()
        {
            if (!File.Exists(Application.dataPath + "/../.gitignore"))
            {
                AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/CopyTemplates/gitignore", ".gitignore");
                File.Delete(Application.dataPath + "/../.gitignore.meta");
            }
        }

        private bool IsValidWorldName(string worldName)
        {
            Regex allowed = new Regex("[^a-zA-Z0-9_]");
            return worldName != "_" && !allowed.IsMatch(worldName);
        }

        private bool CreateWorldsRoot()
        {
            if (!Directory.Exists(GetWorldsRoot(false)))
            {
                Directory.CreateDirectory(GetWorldsRoot(false));
                AssetDatabase.Refresh();

                return true;
            }

            return false;
        }

        private void RestoreTiled()
        {
            string tiledPath = GetWorldsRoot() + "/_Tiled";

            AssetDatabase.DeleteAsset(tiledPath);
            AssetDatabase.Refresh();

            AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/CopyTemplates/Tiled", tiledPath);
            AssetDatabase.Refresh();
        }

        private bool CopySampleWorld()
        {
            if (!Directory.Exists(GetWorldPath(false)))
            {
                AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/CopyTemplates/SampleWorld", GetWorldPath(true));
                AssetDatabase.Refresh();

                return true;
            }

            EditorUtility.DisplayDialog("Error", "This world was already created.", "OK");
            return false;
        }
    }
}