﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Networking;
using WanzyeeStudio.Json;
using Random = System.Random;

namespace traVRsal.SDK
{
    public static class SDKUtil
    {
        public const string PING_ENDPOINT = "www.travrsal.com";
        public const string SERVER_ENDPOINT = "https://www.travrsal.com";

        public const string API_ENDPOINT = SERVER_ENDPOINT + "/api/";

        // public const string API_ENDPOINT = "http://localhost:8000/api/";
        public const int TIMEOUT = 20;

        public const string DEBUG_API_ENDPOINT = "http://localhost:8000/api/";

        public const string PACKAGE_NAME = "com.wetzold.travrsal.sdk";
        public const string AUTO_GENERATED = "[AUTO]";
        public const string EXIT = "[exit]";
        public const string TILED_PATH_DEFAULT = "C:\\Program Files\\Tiled\\tiled.exe";
        public const string LOCKFILE_NAME = "traVRsal.lock";
        public const string MODFILE_NAME = "modding.json";
        public const string STUDIO_INFO_FILE = "traVRsal-studio.json";
        public const string FILE_LISTING = "FileListing.json";

        // TTS
        public const string VOICE_SPEECH_PREFIX = "SPEECH_";
        public const string VOICE_LOADING_WORLD = "LoadingWorld.wav";
        public const string VOICE_LOADING_CHAPTER = "LoadingChapter.wav";
        public const string VOICE_WORLD_LOADED = "Step onto the marker to open the portal, and then walk through!";
        public const string REPLICA_ENDPOINT = "https://api.replicastudios.com/";

        // Predefined pieces
        public const string TRANSITION = "/Base/Transition";
        public const string SCENERY = "/Base/Scenery";

        // only use specified converters to ensure consistent file format
        private static readonly List<JsonConverter> JSON_CONVERTERS = new List<JsonConverter>
        {
            new BoundsConverter(),
            new ColorConverter(),
            new DictionaryConverter(),
            new Matrix4x4Converter(),
            new QuaternionConverter(),
            new RectConverter(),
            new RectOffsetConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Vector4Converter(),
            new StringEnumConverter(),
            new VersionConverter(),
            new HashSetConverter()
        };

        // Layers
        public const int ALWAYS_FRONT_LAYER = 3;
        public const int LOGIC_LAYER = 6;

        // Tags
        public const string INTERACTABLE_TAG = "Interactable";
        public const string COLLECTIBLE_TAG = "Collectible";
        public const string PLAYER_TAG = "Player";
        public const string PLAYER_HEAD_TAG = "Player Head";
        public const string PLAYER_HAND_TAG = "Player Hand";
        public const string PLAYER_HELPER_TAG = "Player Helper";
        public const string ENEMY_TAG = "Enemy";
        public const string RESERVED1_TAG = "Reserved for Future 1";
        public const string RESERVED2_TAG = "Reserved for Future 2";
        public const string RESERVED3_TAG = "Reserved for Future 3";
        public const string RESERVED4_TAG = "Reserved for Future 4";
        public const string RESERVED5_TAG = "Reserved for Future 5";

        public static bool invalidAPIToken;
        public static bool networkIssue;

        public enum ColliderType
        {
            None,
            Box,
            Sphere
        }

        public static float GetDistance(this Transform transform, Transform other)
        {
            return Vector3.Distance(transform.position, other.position);
        }

        public static float GetDistanceAbs(this Transform transform, Transform other)
        {
            return Mathf.Abs(GetDistance(transform, other));
        }

        public static IEnumerator LoadAudioFromFile(string filePath, Action<AudioClip> callback)
        {
            if (!File.Exists(filePath))
            {
                callback(null);
                yield break;
            }

            using UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV);
            ((DownloadHandlerAudioClip) uwr.downloadHandler).streamAudio = true;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip) uwr.downloadHandler;
            if (dlHandler.isDone)
            {
                callback(dlHandler.audioClip);
            }
        }

        public static IEnumerator FetchAPIData<T>(string api, string player, string token, Action<T> callback, Game game, string endPoint = API_ENDPOINT)
        {
            string uri = endPoint + api;
            Debug.Log("Remote (FetchAPIData): " + uri);

            networkIssue = false;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                SetStandardHeaders(webRequest, game);
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
                        yield return DeserializeObjectAsync(webRequest.downloadHandler.text, callback);
                    }
                    yield break;
                }
            }
            callback(default);
        }

        public static void SetStandardHeaders(UnityWebRequest webRequest, Game game)
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("X-Version", Application.version);
            webRequest.SetRequestHeader("X-UnityVersion", Application.unityVersion);
            webRequest.SetRequestHeader("X-Platform", Application.platform.ToString());
            webRequest.SetRequestHeader("X-Do-Not-Track", Application.isEditor ? "True" : "False");
            if (game != null)
            {
                webRequest.SetRequestHeader("X-Channel", game.channel.ToString().ToLowerInvariant());
                webRequest.SetRequestHeader("X-DevMode", game.devMode ? "True" : "False");
            }
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

            T data = DeserializeObject<T>(textFile.text);
            Resources.UnloadAsset(textFile);

            return data;
        }

        public static T ReadJSONFileDirect<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default;

            string text = File.ReadAllText(fileName);
            T data = DeserializeObject<T>(text);

            return data;
        }

        private static JsonSerializerSettings GetDefaultJsonSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = JSON_CONVERTERS
            };
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, GetDefaultJsonSettings());
        }

        public static async Task<T> DeserializeObjectAsync<T>(string json)
        {
            T result = default;
            await Task.Factory.StartNew(() => result = DeserializeObject<T>(json));

            return result;
        }

        public static IEnumerator DeserializeObjectAsync<T>(string json, Action<T> callback)
        {
            Task<T> task = DeserializeObjectAsync<T>(json);
            yield return new WaitUntil(() => task.IsCompleted);
            callback(task.Result);
        }

        // inspired from https://stackoverflow.com/questions/33100164/customize-identation-parameter-in-jsonconvert-serializeobject
        public static string SerializeObject<T>(T value, DefaultValueHandling defaultHandling = DefaultValueHandling.Include, Formatting formatting = Formatting.Indented)
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.DefaultValueHandling = defaultHandling;

            jsonSerializer.Converters.Clear();
            jsonSerializer.Converters.AddRange(JSON_CONVERTERS);

            using JsonTextWriter jsonWriter = new JsonTextWriter(sw);
            jsonWriter.Formatting = formatting;
            jsonWriter.IndentChar = ' ';
            jsonWriter.Indentation = 4;
            jsonSerializer.Serialize(jsonWriter, value, typeof(T));

            return sw.ToString();
        }

        public static async Task<string> SerializeObjectAsync<T>(T value, DefaultValueHandling defaultHandling = DefaultValueHandling.Include, Formatting formatting = Formatting.Indented)
        {
            string result = null;
            await Task.Factory.StartNew(() => result = SerializeObject(value, defaultHandling, formatting));

            return result;
        }

        public static IEnumerator SerializeObjectAsync<T>(T value, Action<string> callback, DefaultValueHandling defaultHandling = DefaultValueHandling.Include, Formatting formatting = Formatting.Indented)
        {
            Task<string> task = SerializeObjectAsync(value);
            yield return new WaitUntil(() => task.IsCompleted);
            callback(task.Result);
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

        public static byte[] GetHash(this string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random _local;

        public static Random ThisThreadsRandom => _local ??= new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
    }
}