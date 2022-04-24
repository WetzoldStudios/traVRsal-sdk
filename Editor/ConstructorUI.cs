using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace traVRsal.SDK
{
    public class ConstructorUI : BasicEditorUI
    {
        private string zoneName;
        private string varName;
        private string worldSetting;
        private string customShader;
        private string fixedSize = "4";
        private static ReplicaVoice[] _replicaVoices;
        private int replicaVoice;

        [MenuItem("traVRsal/Constructor", priority = 110)]
        public static void ShowWindow()
        {
            GetWindow<ConstructorUI>("traVRsal Constructor");
            EditorCoroutineUtility.StartCoroutineOwnerless(FetchReplicaVoices());
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Use the following quick-actions to create often needed entities.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            zoneName = EditorGUILayout.TextField("New Zone:", zoneName);
            if (GUILayout.Button("Create Zone")) ManipulateWorld("AddZone");

            EditorGUILayout.Space();
            varName = EditorGUILayout.TextField("New Variable:", varName);
            if (GUILayout.Button("Create Variable")) ManipulateWorld("AddVariable");

            EditorGUILayout.Space();
            worldSetting = EditorGUILayout.TextField("New World Setting:", worldSetting);
            if (GUILayout.Button("Create World Setting")) ManipulateWorld("AddSetting");

            if (_replicaVoices != null)
            {
                EditorGUILayout.Space();
                replicaVoice = EditorGUILayout.Popup("New Replica Voice", replicaVoice, _replicaVoices.Select(v => v.name).ToArray());
                if (GUILayout.Button("Add Voice")) ManipulateWorld("AddReplicaVoice");
            }

            EditorGUILayout.Space();
            customShader = EditorGUILayout.TextField("Shader Name:", customShader);
            if (GUILayout.Button("Add Custom Shader")) ManipulateWorld("AddCustomShader");

            EditorGUILayout.Space(20f);
            GUILayout.Label("Misc", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            fixedSize = EditorGUILayout.TextField("Fixed Tile Count:", fixedSize);
            if (GUILayout.Button("Set Fixed Count")) ManipulateWorld("SetFixedTileCount");

            EditorGUILayout.Space();
            if (GUILayout.Button("Set Minimum World Version to Current SDK Version")) ManipulateWorld("SetToCurrentVersion");

            OnGUIDone();
        }

        private void ManipulateWorld(string command)
        {
            World world = LoadWorld();
            if (world == null) return;

            switch (command)
            {
                case "SetToCurrentVersion":
                    PackageInfo pi = PackageInfo.FindForAssetPath("Packages/com.wetzold.travrsal.sdk/package.json");
                    if (pi != null)
                    {
                        world.minAppVersion = pi.version;
                        EditorUtility.DisplayDialog("Success", $"Set minimum app version required to play the world to {pi.version}.", "OK");
                    }
                    break;

                case "AddZone":
                    if (string.IsNullOrEmpty(zoneName)) return;
                    string root = GetWorldPaths()[0];
                    string path = $"{root}/Data/{zoneName}.tmx";
                    if (File.Exists(path))
                    {
                        EditorUtility.DisplayDialog("Error", "A zone with that name already exists.", "OK");
                        return;
                    }
                    AssetDatabase.CopyAsset($"Packages/{SDKUtil.PACKAGE_NAME}/Editor/CopyTemplates/EmptyZone.copy", $"Assets/Worlds/{world.key}/Data/{zoneName}.tmx");

                    // two cases: either world explicitly lists files already or default is used with config.world mechanism 
                    if (world.worldData != null && world.worldData.Count > 0)
                    {
                        world.worldData.Add(new WorldDataReference(path, WorldDataReference.ImportType.TileMap));
                    }
                    else
                    {
                        string mapFile = root + "/Data/Config.world";
                        TMWorld worldMap = SDKUtil.ReadJSONFileDirect<TMWorld>(mapFile);
                        if (worldMap != null)
                        {
                            worldMap.maps = worldMap.maps.Append(new WorldMap(Path.GetFileName(path)));

                            File.WriteAllText(mapFile, SDKUtil.SerializeObject(worldMap));
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Data/Config.world file for {world.key} does not exist. New zone was created but needs to be manually linked.", "OK");
                        }
                    }
                    zoneName = "";
                    AssetDatabase.Refresh();
                    break;

                case "AddVariable":
                    if (string.IsNullOrEmpty(varName)) return;
                    world.initialVariables ??= new List<Variable>();
                    world.initialVariables.Add(new Variable(varName));
                    varName = "";
                    break;

                case "AddSetting":
                    if (string.IsNullOrEmpty(worldSetting)) return;
                    world.settings ??= new List<WorldSetting>();
                    world.settings.Add(new WorldSetting(worldSetting));
                    worldSetting = "";
                    break;

                case "AddReplicaVoice":
                    world.voices ??= new List<VoiceSpec>();
                    world.voices.Add(new VoiceSpec(VoiceSpec.TTSBackend.Replica, _replicaVoices[replicaVoice].name, _replicaVoices[replicaVoice].uuid));
                    break;

                case "AddCustomShader":
                    if (string.IsNullOrEmpty(customShader)) return;
                    world.customShaders ??= new List<string>();
                    world.customShaders.Add(customShader);
                    customShader = "";
                    break;

                case "SetFixedTileCount":
                    if (string.IsNullOrEmpty(fixedSize)) return;
                    int size = int.Parse(fixedSize);
                    if (size <= 0) return;
                    world.minSize = $"{size},{size}";
                    world.maxSize = $"{size},{size}";
                    fixedSize = "";
                    break;
            }

            SaveWorld(world);
        }

        private static World LoadWorld()
        {
            string[] paths = GetWorldPaths();
            if (paths == null || paths.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "There are no worlds yet. Use the setup tool to create one.", "OK");
                return null;
            }
            if (paths.Length > 1)
            {
                EditorUtility.DisplayDialog("Error", "There is more than one world in the project. The constructor does not support multi-world projects yet.", "OK");
                return null;
            }

            string worldName = Path.GetFileName(paths[0]);
            string root = GetWorldsRoot() + "/" + worldName + "/";
            World world = SDKUtil.ReadJSONFileDirect<World>(root + "World.json");
            if (world == null)
            {
                EditorUtility.DisplayDialog("Error", $"World.json file for {worldName} seems corrupted and needs to be fixed first.", "OK");
                return null;
            }
            world.key = worldName;

            return world;
        }

        private static void SaveWorld(World world)
        {
            string[] paths = GetWorldPaths();
            string worldName = Path.GetFileName(paths[0]);
            string root = GetWorldsRoot() + "/" + worldName + "/";

            world.NullifyEmpties();
            File.WriteAllText(root + "World.json", SDKUtil.SerializeObject(world, DefaultValueHandling.Ignore));
        }

        private static IEnumerator FetchReplicaVoices()
        {
            Debug.Log("Remote (Fetch Replica Voices)");

            if (_replicaToken == null) yield return GetReplicaToken();
            if (_replicaToken != null)
            {
                using UnityWebRequest webRequest = UnityWebRequest.Get(SDKUtil.REPLICA_ENDPOINT + "voices");
                webRequest.SetRequestHeader("Authorization", "Bearer " + _replicaToken);
                webRequest.timeout = SDKUtil.TIMEOUT;
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError($"Could not fetch Replica voices due to network issues: {webRequest.error}");
                }
                else if (webRequest.isHttpError)
                {
                    if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                    {
                        Debug.LogError($"Invalid or expired API Token when contacting Replica");
                    }
                    else
                    {
                        Debug.LogError($"There was an error fetching data: {webRequest.downloadHandler.text}");
                    }
                }
                else
                {
                    _replicaVoices = SDKUtil.DeserializeObject<ReplicaVoice[]>(webRequest.downloadHandler.text)
                        .Where(rv => !rv.name.StartsWith("(old model)"))
                        .ToArray();
                }
            }
        }
    }
}