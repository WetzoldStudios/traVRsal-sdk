using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class SetupUI : BasicEditorUI
    {
        private string worldName;

        [MenuItem("traVRsal/Setup", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<SetupUI>("traVRsal Setup");
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Create your own worlds and amaze other players! A sample world will help you to get started.", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            worldName = EditorGUILayout.TextField("World Key: ", worldName);
            if (GUILayout.Button("Create New World")) CreateSampleWorld();

            GUILayout.Space(10);
            GUILayout.Label("Maintenance Functions", EditorStyles.boldLabel);
            if (GUILayout.Button("Update/Restore Tiled Data")) RestoreTiled();

            OnGUIDone();
        }

        private string GetWorldPath(bool relative)
        {
            return GetWorldsRoot(relative) + "/" + worldName;
        }

        private void CreateSampleWorld()
        {
            if (string.IsNullOrEmpty(worldName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "No world key specified.", "OK");
                return;
            }
            if (!IsValidWorldName(worldName))
            {
                EditorUtility.DisplayDialog("Invalid Entry", "World key is not valid: must be upper and lower case characters, numbers and undercore only.", "OK");
                return;
            }

            if (CreateWorldsRoot()) RestoreTiled();
            CopySampleWorld();

            EditorUtility.FocusProjectWindow();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetWorldPath(true));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);

            worldName = "";
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
            else
            {
                return false;
            }
        }

        private void RestoreTiled()
        {
            string tiledPath = GetWorldsRoot(true) + "/_Tiled";

            AssetDatabase.DeleteAsset(tiledPath);
            AssetDatabase.Refresh();

            AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/_Tiled", tiledPath);
            AssetDatabase.Refresh();
        }

        private bool CopySampleWorld()
        {
            if (!Directory.Exists(GetWorldPath(false)))
            {
                AssetDatabase.CopyAsset("Packages/" + SDKUtil.PACKAGE_NAME + "/Editor/_World", GetWorldPath(true));
                AssetDatabase.Refresh();

                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "World with identical name already exists.", "OK");

                return false;
            }
        }
    }
}