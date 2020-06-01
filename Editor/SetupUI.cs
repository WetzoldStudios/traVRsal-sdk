using System.IO;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class SetupUI : EditorWindow
    {
        private string levelName;
        private static GUIStyle logo;

        [MenuItem("traVRsal/Setup", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow<SetupUI>("traVRsal Setup");
        }

        void OnEnable()
        {
            Texture2D logoImage = null;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Packages/com.wetzold.travrsal.sdk/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Assets/SDK/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;

            logo = new GUIStyle { normal = { background = logoImage }, fixedWidth = 128, fixedHeight = 64 };
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("", logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Levels are the basic building block. Single levels can either be shared with others on traVRsal or combined with other levels into your own game.", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            levelName = EditorGUILayout.TextField("Level Name: ", levelName);
            if (GUILayout.Button("Create New Level")) CreateSampleLevel();

            GUILayout.Space(10);
            GUILayout.Label("Tiled data evolves over time with new icons and options. If you didn't make changes to the Tiled data yourself (recommended), you should update it with every new SDK release. This will copy the newest version to your project.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Update/Restore Tiled")) RestoreTiled();
        }

        private string GetLevelsRoot()
        {
            return Application.dataPath + "/Levels";
        }

        private string GetLevelPath()
        {
            return GetLevelsRoot() + "/" + levelName;
        }

        private void CreateSampleLevel()
        {
            if (CreateLevelsRoot()) RestoreTiled();
            CreateSampleWorld();
        }

        private bool CreateLevelsRoot()
        {
            if (!Directory.Exists(GetLevelsRoot()))
            {
                Directory.CreateDirectory(GetLevelsRoot());
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
            string tiledPath = GetLevelsRoot() + "/_Tiled";

            if (Directory.Exists(tiledPath)) Directory.Delete(tiledPath, true);
            AssetDatabase.Refresh();

            string id = AssetDatabase.FindAssets("_Tiled")[0];
            string path = AssetDatabase.GUIDToAssetPath(id);
            AssetDatabase.CopyAsset(path, tiledPath);
            AssetDatabase.Refresh();
        }

        private bool CreateSampleWorld()
        {
            if (!Directory.Exists(GetLevelPath()))
            {
                string id = AssetDatabase.FindAssets("_Level")[0];
                string path = AssetDatabase.GUIDToAssetPath(id);
                AssetDatabase.CopyAsset(path, GetLevelPath());
                AssetDatabase.Refresh();

                return true;
            }
            else
            {
                Debug.LogError("Level with identical name already exists.");
                return false;
            }
        }
    }
}