using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace traVRsal.SDK
{
    public static class SDKUtil
    {
        public const string PING_ENDPOINT = "www.travrsal.com";
        public const string SERVER_ENDPOINT = "https://www.travrsal.com";
        public const string API_ENDPOINT = SERVER_ENDPOINT + "/api/";
        public const int TIMEOUT = 20;

        // public const string API_ENDPOINT = "http://localhost:8000/api/";
        public const string DEBUG_API_ENDPOINT = "http://localhost:8000/api/";

        public const string PACKAGE_NAME = "com.wetzold.travrsal.sdk";
        public const string AUTO_GENERATED = "[AUTO]";
        public const string TILED_PATH_DEFAULT = "C:\\Program Files\\Tiled\\tiled.exe";
        public const string LOCKFILE_NAME = "traVRsal.lock";
        public const string MODFILE_NAME = "modding.json";

        // Tags
        public const string INTERACTABLE_TAG = "Interactable";
        public const string COLLECTIBLE_TAG = "Collectible";
        public const string PLAYER_TAG = "Player";
        public const string PLAYER_HEAD_TAG = "Player Head";
        public const string PLAYER_HELPER_TAG = "Player Helper";
        public const string ENEMY_TAG = "Enemy";

        public static bool invalidAPIToken;
        public static bool networkIssue;

        public enum ColliderType
        {
            None,
            Box,
            Sphere
        }

        public static IEnumerator FetchAPIData<T>(string api, string player, string token, Action<T> callback, string endPoint = API_ENDPOINT)
        {
            string uri = endPoint + api;
            Debug.Log("Remote (FetchAPIData): " + uri);

            networkIssue = false;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                SetStandardHeaders(webRequest);
                webRequest.SetRequestHeader("Authorization", "Bearer " + token);
                webRequest.timeout = TIMEOUT;
                if (!string.IsNullOrEmpty(player)) webRequest.SetRequestHeader("X-Player", player);
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    networkIssue = true;
                    Debug.LogError($"Could not fetch data due to network issues: {webRequest.error}");
                }
                else if (webRequest.isHttpError)
                {
                    if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                    {
                        invalidAPIToken = true;
                        Debug.LogError($"Invalid or expired API Token when contacting {uri}");
                    }
                    else
                    {
                        Debug.LogError($"There was an error fetching data: {webRequest.downloadHandler.text}");
                    }
                }
                else
                {
                    invalidAPIToken = false;
                    if (typeof(T) == typeof(string))
                    {
                        callback((T) Convert.ChangeType(webRequest.downloadHandler.text, typeof(T)));
                    }
                    else
                    {
                        callback(JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }));
                    }
                    yield break;
                }
            }
            callback(default);
        }

        public static void SetStandardHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("X-Version", Application.version);
            webRequest.SetRequestHeader("X-Do-Not-Track", Application.isEditor ? "True" : "False");
        }

        public static TMProperty[] CopyProperties(TMProperty[] copyFrom)
        {
            if (copyFrom == null) return null;

            TMProperty[] properties = new TMProperty[copyFrom.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = new TMProperty(copyFrom[i].name, copyFrom[i].type, copyFrom[i].value);
            }

            return properties;
        }

        public static T ReadJSONFile<T>(string fileName)
        {
            TextAsset textFile = (TextAsset) Resources.Load(fileName);
            if (textFile == null) return default;

            T data = JsonConvert.DeserializeObject<T>(textFile.text);
            Resources.UnloadAsset(textFile);

            return data;
        }

        public static T ReadJSONFileDirect<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default;

            string text = File.ReadAllText(fileName);
            T data = JsonConvert.DeserializeObject<T>(text);

            return data;
        }

        // inspired from https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        public static string BytesToString(long byteCount)
        {
            string[] suffix = {"B", "KB", "MB", "GB", "TB", "PB", "EB"};
            if (byteCount == 0) return "0" + suffix[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + suffix[place];
        }

        public static string MaxLength(string text, int maxLength)
        {
            return text.Length < maxLength ? text : text.Substring(0, maxLength);
        }

        // inspired from https://stackoverflow.com/questions/33100164/customize-identation-parameter-in-jsonconvert-serializeobject
        public static string SerializeObject<T>(T value)
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.IndentChar = ' ';
                jsonWriter.Indentation = 4;

                jsonSerializer.Serialize(jsonWriter, value, typeof(T));
            }

            return sw.ToString();
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
            {
                action(obj);
            }
            return source;
        }

        public static T[] Append<T>(this T[] data, T element)
        {
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = element;

            return data;
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom => Local ??= new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
    }
}