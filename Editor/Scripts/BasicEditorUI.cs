using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace traVRsal.SDK
{
    public abstract class BasicEditorUI : EditorWindow
    {
        public UserWorld[] userWorlds;

        protected static string _replicaToken;
        private static GUIStyle logo;
        private Vector2 windowScrollPos;

        public virtual void OnEnable()
        {
            string logoName = EditorGUIUtility.isProSkin ? "travrsal-white-300.png" : "travrsal-300.png";

            Texture2D logoImage = AssetDatabase.LoadAssetAtPath($"Packages/com.wetzold.travrsal.sdk/Editor/Images/{logoName}", typeof(Texture2D)) as Texture2D;
            if (logoImage == null) logoImage = AssetDatabase.LoadAssetAtPath($"Assets/SDK/Editor/Images/{logoName}", typeof(Texture2D)) as Texture2D;

            logo = new GUIStyle {normal = {background = logoImage}, fixedWidth = 128, fixedHeight = 64};

            // perform (cheap) setup tasks
            SetupTagsAndLayers();

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

        private static void SetupTagsAndLayers()
        {
            Dictionary<int, string> requiredLayers = new Dictionary<int, string>
            {
                {SDKUtil.ALWAYS_FRONT_LAYER, "Always In Front"},
                {SDKUtil.LOGIC_LAYER, "Logic"}
            };

            List<string> requiredTags = new List<string>
            {
                "ExcludeTeleport", SDKUtil.INTERACTABLE_TAG, SDKUtil.ENEMY_TAG, SDKUtil.PLAYER_HEAD_TAG, SDKUtil.COLLECTIBLE_TAG, SDKUtil.PLAYER_HELPER_TAG, SDKUtil.PLAYER_HAND_TAG, SDKUtil.RESERVED1_TAG, SDKUtil.RESERVED2_TAG, SDKUtil.RESERVED3_TAG, SDKUtil.RESERVED4_TAG, SDKUtil.RESERVED5_TAG
            };
            Enumerable.Range(1, 100).ForEach(i => requiredTags.Add("Object " + i));

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            SerializedProperty layersProp = tagManager.FindProperty("layers");

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

            // override layers
            foreach (KeyValuePair<int, string> layer in requiredLayers)
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(layer.Key);
                sp.stringValue = layer.Value;
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
            return Array.Empty<string>();
        }

        protected IEnumerator FetchUserWorlds()
        {
            yield return SDKUtil.FetchAPIData<UserWorld[]>("userworlds", null, GetAPIToken(), worlds =>
            {
                if (worlds != null) userWorlds = worlds;
            }, null);
        }

        protected static string GetAPIToken()
        {
            return TravrsalSettingsManager.Get("apiKey", "");
        }

        protected static IEnumerator GetReplicaToken()
        {
            string login = TravrsalSettingsManager.Get("replicaLogin", "");
            string password = TravrsalSettingsManager.Get("replicaPassword", "");
            if (string.IsNullOrWhiteSpace(login)) yield break; // nothing configured

            Debug.Log("Remote (Get Replica Token)");

            WWWForm form = new WWWForm();
            form.AddField("client_id", login);
            form.AddField("secret", password);

            using UnityWebRequest webRequest = UnityWebRequest.Post(SDKUtil.REPLICA_ENDPOINT + "auth", form);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Replica authentication error: " + webRequest.error);
            }
            else
            {
                _replicaToken = SDKUtil.DeserializeObject<ReplicaAuth>(webRequest.downloadHandler.text).access_token;
                Debug.Log("Replica Token: " + _replicaToken);
            }
        }

        protected static IEnumerator FetchReplicaSpeech(string text, string speaker, string filePath, Action<bool> callback)
        {
            Debug.Log("Remote (Fetch Replica Speech)");

            if (_replicaToken == null) yield return GetReplicaToken();
            if (_replicaToken != null)
            {
                string escapedText = UnityWebRequest.EscapeURL(text);
                using UnityWebRequest webRequest = UnityWebRequest.Get($"{SDKUtil.REPLICA_ENDPOINT}speech?txt={escapedText}&speaker_id={speaker}&extension=wav&bit_rate=128&sample_rate=22050");
                webRequest.SetRequestHeader("Authorization", "Bearer " + _replicaToken);
                webRequest.timeout = SDKUtil.TIMEOUT;
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError($"Could not fetch Replica speech due to network issues: {webRequest.error}");
                }
                else if (webRequest.isHttpError)
                {
                    if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                    {
                        Debug.LogError("Invalid or expired API Token when contacting Replica");
                    }
                    else
                    {
                        Debug.LogError($"There was an error fetching speech data: {webRequest.downloadHandler.text}");
                    }
                }
                else
                {
                    ReplicaSpeech result = SDKUtil.DeserializeObject<ReplicaSpeech>(webRequest.downloadHandler.text);
                    Debug.Log(" Speech duration: " + result.duration);

                    // download actual file
                    using UnityWebRequest fileRequest = UnityWebRequest.Get(result.url);
                    DownloadHandlerFile dh = new DownloadHandlerFile(filePath);
                    dh.removeFileOnAbort = true;
                    fileRequest.downloadHandler = dh;
                    yield return fileRequest.SendWebRequest();
                    if (fileRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Could not fetch Replica speech file: {fileRequest.error}");
                    }
                    else
                    {
                        Debug.Log(" Saved to: " + filePath);
                        callback?.Invoke(true);
                        yield break;
                    }
                }
            }

            callback?.Invoke(false);
        }
    }
}