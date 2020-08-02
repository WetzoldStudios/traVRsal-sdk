using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class SetupUI : BasicEditorUI
    {
        private string levelName;

        [MenuItem("traVRsal/Setup", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<SetupUI>("traVRsal Setup");
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Levels are the basic building block. Single levels can either be shared with others on traVRsal or combined with other levels into your own game.", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            levelName = EditorGUILayout.TextField("Level Key: ", levelName);
            if (GUILayout.Button("Create New Level")) CreateSampleLevel();

            GUILayout.Space(10);
            GUILayout.Label("Maintenance Functions", EditorStyles.boldLabel);
            if (GUILayout.Button("Update/Restore Tiled Data")) RestoreTiled();

            OnGUIDone();
        }

        private string GetLevelPath(bool relative)
        {
            return GetLevelsRoot(relative) + "/" + levelName;
        }

        private void CreateSampleLevel()
        {
            if (string.IsNullOrEmpty(levelName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "No level key specified.", "OK");
                return;
            }
            if (!IsValidLevelName(levelName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "Level key is not valid: must be upper and lower case characters, numbers and undercore only.", "OK");
                return;
            }

            if (CreateLevelsRoot()) RestoreTiled();
            CreateSampleWorld();

            EditorUtility.FocusProjectWindow();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetLevelPath(true));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);

            levelName = "";
        }

        private bool IsValidLevelName(string levelName)
        {
            Regex allowed = new Regex("[^a-zA-Z0-9_]");
            return levelName != "_" && !allowed.IsMatch(levelName);
        }

        private bool CreateLevelsRoot()
        {
            if (!Directory.Exists(GetLevelsRoot(false)))
            {
                Directory.CreateDirectory(GetLevelsRoot(false));
                AssetDatabase.Refresh();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void RestoreTiled()
        {
            string tiledPath = GetLevelsRoot(true) + "/_Tiled";

            AssetDatabase.DeleteAsset(tiledPath);
            AssetDatabase.Refresh();

            AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/_Tiled", tiledPath);
            AssetDatabase.Refresh();
        }

        private bool CreateSampleWorld()
        {
            if (!Directory.Exists(GetLevelPath(false)))
            {
                AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/_Level", GetLevelPath(true));
                AssetDatabase.Refresh();

                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Level with identical name already exists.", "OK");

                return false;
            }
        }
    }
}