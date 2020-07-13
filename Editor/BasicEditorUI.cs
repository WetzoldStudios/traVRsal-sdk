using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public abstract class BasicEditorUI : EditorWindow
    {
        private string[] REQUIRED_TAGS = { "ExcludeTeleport", "Interactable", "Enemy", "Fire", "Collectible" };

        private static GUIStyle logo;
        private Vector2 scrollPos;

        public virtual void OnEnable()
        {
            Texture2D logoImage = null;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Packages/com.wetzold.travrsal.sdk/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Assets/SDK/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;

            logo = new GUIStyle { normal = { background = logoImage }, fixedWidth = 128, fixedHeight = 64 };

            // perform (cheap) setup tasks
            SetupTags();
        }

        public virtual void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("", logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        public void OnGUIDone()
        {
            GUILayout.EndScrollView();
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

        public string GetLevelsRoot()
        {
            return Application.dataPath + "/Levels";
        }
    }
}