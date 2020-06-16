using System.IO;
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
            levelName = EditorGUILayout.TextField("Level Name: ", levelName);
            if (GUILayout.Button("Create New Level")) CreateSampleLevel();

            GUILayout.Space(10);
            GUILayout.Label("Tiled data evolves over time with new icons and options. If you didn't make changes to the Tiled data yourself (recommended), you should update it with every new SDK release. This will copy the newest version to your project.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Update/Restore Tiled")) RestoreTiled();
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