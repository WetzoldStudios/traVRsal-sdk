using Newtonsoft.Json;
using System.Collections;
using System.Net;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace traVRsal.SDK
{
    public abstract class BasicEditorUI : EditorWindow
    {
        private string[] REQUIRED_TAGS = { "ExcludeTeleport", SDKUtil.INTERACTABLE_TAG, SDKUtil.ENEMY_TAG, SDKUtil.PLAYER_HEAD_TAG, SDKUtil.COLLECTIBLE_TAG, SDKUtil.PLAYER_HELPER_TAG };
        private string API_ENDPOINT = "http://localhost:8000/api/";

        public UserWorld[] userWorlds;
        public bool invalidAPIToken = false;

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

            EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
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

        public string GetWorldsRoot(bool relative)
        {
            return (relative ? "Assets" : Application.dataPath) + "/Worlds";
        }

        public string GetAPIToken()
        {
            return TravrsalSettingsManager.Get("apiKey", "");
        }

        public IEnumerator FetchUserWorlds(bool silent = true)
        {
            string uri = API_ENDPOINT + "worlds";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GetAPIToken());
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("Could not fetch worlds due to network issues: " + webRequest.error);
                }
                else if (webRequest.isHttpError)
                {
                    if (webRequest.responseCode == (int)HttpStatusCode.Unauthorized)
                    {
                        invalidAPIToken = true;
                        Debug.LogError("Invalid or expired API Token.");
                    }
                    else
                    {
                        Debug.LogError("There was an error when fetching worlds: " + webRequest.error);
                    }
                }
                else
                {
                    userWorlds = JsonConvert.DeserializeObject<UserWorld[]>(webRequest.downloadHandler.text);
                    invalidAPIToken = false;
                }
            }
        }
    }
}