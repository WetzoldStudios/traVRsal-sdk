using System.IO;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class SetupUI : BasicEditorUI
    {
        private string[] REQUIRED_TAGS = { "ExcludeTeleport", "Interactable", "Enemy", "Fire", "Collectible" };

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
            GUILayout.Label("The following actions should be performed once after an update of the SDK was installed, since the framework constantly evolves.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Setup Tags")) SetupTags();
            if (GUILayout.Button("Update/Restore Tiled Data")) RestoreTiled();
        }

        private void SetupTags()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // delete all tags first
            for (int i = tagsProp.arraySize - 1; i >= 0; i--)
            {
                tagsProp.DeleteArrayElementAtIndex(i);
            }

            // recreate in exact order required
            for (int i = 0; i < REQUIRED_TAGS.Length; i++)
            {
                tagsProp.InsertArrayElementAtIndex(i);
                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(i);
                newTag.stringValue = REQUIRED_TAGS[i];
            }

            tagManager.ApplyModifiedProperties();
        }

        private string GetLevelPath()
        {
            return GetLevelsRoot() + "/" + levelName;
        }

        private void CreateSampleLevel()
        {
            if (string.IsNullOrEmpty(levelName))
            {
                Debug.LogError("No level name specified.");
                return;
            }

            if (CreateLevelsRoot()) RestoreTiled();
            CreateSampleWorld();

            EditorUtility.FocusProjectWindow();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Levels/" + levelName);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);

            levelName = "";
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