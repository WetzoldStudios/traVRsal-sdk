using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public abstract class BasicEditorUI : EditorWindow
    {
        public UserWorld[] userWorlds;

        private static GUIStyle logo;
        private Vector2 windowScrollPos;

        public virtual void OnEnable()
        {
            string logoName = EditorGUIUtility.isProSkin ? "travrsal-white-300.png" : "travrsal-300.png";

            Texture2D logoImage = AssetDatabase.LoadAssetAtPath($"Packages/com.wetzold.travrsal.sdk/Editor/Images/{logoName}", typeof(Texture2D)) as Texture2D;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath($"Assets/SDK/Editor/Images/{logoName}", typeof(Texture2D)) as Texture2D;

            logo = new GUIStyle {normal = {background = logoImage}, fixedWidth = 128, fixedHeight = 64};

            // perform (cheap) setup tasks
            SetupTags();

            EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
        }

        public virtual void OnGUI()
        {
            windowScrollPos = GUILayout.BeginScrollView(windowScrollPos, false, false);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("", logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public void OnGUIDone()
        {
            GUILayout.EndScrollView();
        }

        protected bool CheckTokenGUI()
        {
            if (string.IsNullOrEmpty(GetAPIToken()))
            {
                EditorGUILayout.HelpBox("You have not entered your Creator Key yet. Please do so in the Project Settings. You can find your personal key on www.traVRsal.com.", MessageType.Error);
                if (GUILayout.Button("Visit traVRsal.com")) Help.BrowseURL("https://www.traVRsal.com/home");

                return false;
            }
            if (SDKUtil.invalidAPIToken)
            {
                EditorGUILayout.HelpBox("Your Creator Key is invalid or expired. Please update it in the Project Settings. You can find your personal key on www.traVRsal.com.", MessageType.Error);
                if (GUILayout.Button("Retry")) EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
                if (GUILayout.Button("Visit traVRsal.com")) Help.BrowseURL("https://www.traVRsal.com/home");

                return false;
            }
            if (SDKUtil.networkIssue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("There are issues connecting to the server.", MessageType.Error);
                if (GUILayout.Button("Retry")) EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
                return false;
            }

            return true;
        }

        private void SetupTags()
        {
            List<string> requiredTags = new List<string> {"ExcludeTeleport", SDKUtil.INTERACTABLE_TAG, SDKUtil.ENEMY_TAG, SDKUtil.PLAYER_HEAD_TAG, SDKUtil.COLLECTIBLE_TAG, SDKUtil.PLAYER_HELPER_TAG};
            Enumerable.Range(1, 100).ForEach(i => requiredTags.Add("Object " + i));

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // delete all tags first
            for (int i = tagsProp.arraySize - 1; i >= 0; i--)
            {
                tagsProp.DeleteArrayElementAtIndex(i);
            }

            // recreate in exact order required
            for (int i = 0; i < requiredTags.Count; i++)
            {
                tagsProp.InsertArrayElementAtIndex(i);
                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(i);
                newTag.stringValue = requiredTags[i];
            }

            tagManager.ApplyModifiedProperties();
        }

        protected static string GetWorldsRoot(bool relative = true)
        {
            return (relative ? "Assets" : Application.dataPath) + "/Worlds";
        }

        protected static string[] GetWorldPaths()
        {
            if (Directory.Exists(Application.dataPath + "/Worlds"))
            {
                return Directory.GetDirectories(Application.dataPath + "/Worlds").Where(s => !Path.GetFileName(s).StartsWith("_")).ToArray();
            }
            return new string[0];
        }

        protected string GetAPIToken()
        {
            return TravrsalSettingsManager.Get("apiKey", "");
        }

        protected IEnumerator FetchUserWorlds()
        {
            yield return SDKUtil.FetchAPIData<UserWorld[]>("userworlds", null, GetAPIToken(), worlds =>
            {
                if (worlds != null) userWorlds = worlds;
            }, null);
        }
    }
}