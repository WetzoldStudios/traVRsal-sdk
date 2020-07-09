using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public abstract class BasicEditorUI : EditorWindow
    {
        private static GUIStyle logo;
        private Vector2 scrollPos;

        public virtual void OnEnable()
        {
            Texture2D logoImage = null;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Packages/com.wetzold.travrsal.sdk/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath("Assets/SDK/Editor/Images/travrsal-300.png", typeof(Texture2D)) as Texture2D;

            logo = new GUIStyle { normal = { background = logoImage }, fixedWidth = 128, fixedHeight = 64 };
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

        public string GetLevelsRoot()
        {
            return Application.dataPath + "/Levels";
        }
    }
}