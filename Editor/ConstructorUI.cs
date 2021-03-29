using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace traVRsal.SDK
{
    public class ConstructorUI : BasicEditorUI
    {
        private string zoneName;
        private string varName;
        private string worldSetting;
        private string customShader;

        [MenuItem("traVRsal/Constructor", priority = 110)]
        public static void ShowWindow()
        {
            GetWindow<ConstructorUI>("traVRsal Constructor");
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

            EditorGUILayout.Space();
            customShader = EditorGUILayout.TextField("Shader Name:", customShader);
            if (GUILayout.Button("Add Custom Shader")) ManipulateWorld("AddCustomShader");

            EditorGUILayout.Space(20f);
            GUILayout.Label("Misc", EditorStyles.boldLabel);

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
                        world.minVersion = pi.version;
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
                    if (world.initialVariables == null) world.initialVariables = new List<Variable>();
                    world.initialVariables.Add(new Variable(varName));
                    varName = "";
                    break;

                case "AddSetting":
                    if (string.IsNullOrEmpty(worldSetting)) return;
                    if (world.settings == null) world.settings = new List<WorldSetting>();
                    world.settings.Add(new WorldSetting(worldSetting));
                    worldSetting = "";
                    break;

                case "AddCustomShader":
                    if (string.IsNullOrEmpty(customShader)) return;
                    if (world.customShaders == null) world.customShaders = new List<string>();
                    world.customShaders.Add(customShader);
                    customShader = "";
                    break;
            }

            SaveWorld(world);
        }

        private World LoadWorld()
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

        private void SaveWorld(World world)
        {
            string[] paths = GetWorldPaths();
            string worldName = Path.GetFileName(paths[0]);
            string root = GetWorldsRoot() + "/" + worldName + "/";

            world.NullifyEmpties();
            File.WriteAllText(root + "World.json", SDKUtil.SerializeObject(world));
        }
    }
}