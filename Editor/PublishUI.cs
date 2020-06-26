using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace traVRsal.SDK
{
    public class PublishUI : BasicEditorUI
    {
        public const string LOCKFILE_NAME = "traVRsal.lock";

        private string[] PACKAGE_OPTIONS = { "Everything", "Intelligent", "Server" };

        private bool packagingInProgress = false;
        private bool documentationInProgress = false;
        private bool uploadInProgress = false;
        private DateTime uploadStartTime;
        private float uploadProgress = 1;
        private int packageMode = 1;
        private static DirectoryWatcher dirWatcher;

        [MenuItem("traVRsal/Publisher", priority = 110)]
        public static void ShowWindow()
        {
            GetWindow<PublishUI>("traVRsal Publisher");
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (dirWatcher == null)
            {
                dirWatcher = new DirectoryWatcher(new FSWParams(Application.dataPath + "/Levels"));
                dirWatcher.StartFSW();
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Label("Packaging ensures that the editor shows the most up to date version of your level.", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Packaging Mode:", EditorStyles.wordWrappedLabel);
            packageMode = GUILayout.SelectionGrid(packageMode, PACKAGE_OPTIONS, 3, EditorStyles.radioButton);
            GUILayout.EndHorizontal();

            if (packagingInProgress)
            {
                GUILayout.Label("Packaging in progress...", EditorStyles.boldLabel);
            }
            else if (uploadInProgress)
            {
                GUILayout.Label("Packaging not possible during upload", EditorStyles.boldLabel);
            }
            else
            {
                string buttonText = "Package";
                if (packageMode == 1)
                {
                    string[] levelsToBuild = GetLevelsToBuild().Select(path => Path.GetFileName(path)).ToArray();
                    buttonText += " (" + ((dirWatcher.affectedFiles.Count > 0) ? string.Join(", ", levelsToBuild) : "everything") + ")";
                }
                if (GUILayout.Button(buttonText)) PackageLevels(packageMode == 2 ? true : false, packageMode == 2 ? true : false);
            }

            if (documentationInProgress)
            {
                GUILayout.Label("Documentation creation in progress...", EditorStyles.boldLabel);
            }
            else
            {
                if (GUILayout.Button("Create Documentation (Experimental)")) EditorCoroutineUtility.StartCoroutine(CreateDocumentation(), this);
            }
            if (GUILayout.Button("Reimport all Tiled files")) ReimportTiled();

            GUILayout.Space(10);
            GUILayout.Label("Once a level or game is done, it can be packaged and sent to the central server for distribution. It will do an automatic full packaging step before.", EditorStyles.wordWrappedLabel);
            if (uploadInProgress)
            {
                GUILayout.Label("Upload in progress...", EditorStyles.boldLabel);
            }
            else if (packagingInProgress)
            {
                GUILayout.Label("Upload not possible during packaging", EditorStyles.boldLabel);
            }
            else
            {
                if (GUILayout.Button("Upload")) UploadLevels();
            }

            int timeRemaining = Mathf.Max(1, Mathf.RoundToInt((DateTime.Now.Subtract(uploadStartTime).Seconds / uploadProgress) * (1 - uploadProgress)));
            if (uploadInProgress) EditorUtility.DisplayProgressBar("Progress", "Uploading levels to server... " + timeRemaining + "s", uploadProgress);
        }

        private void ReimportTiled()
        {
            List<string> allfiles = DirectoryUtil.GetFiles(Application.dataPath, new string[] { "*.world", "*.tmx" }, SearchOption.AllDirectories).ToList();
            TileMapConverter.ConvertTiledToJSON(allfiles);
        }

        private string GetServerDataPath()
        {
            return Application.dataPath + "/../ServerData";
        }

        private string[] GetLevelsToBuild()
        {
            string[] levelsToBuild = GetLevelPaths();
            if (packageMode == 1)
            {
                try
                {
                    string[] filteredLevels = levelsToBuild.Where(
                        levelName => dirWatcher.affectedFiles.Where(
                            af => af.Contains("/" + Path.GetFileName(levelName) + "/") || af.Contains("\\" + Path.GetFileName(levelName) + "\\")).Any()
                        ).ToArray();
                    if (filteredLevels.Length > 0) levelsToBuild = filteredLevels;
                }
                catch { }
            }

            return levelsToBuild;
        }

        private void PackageLevels(bool allLevels, bool allTargets)
        {
            CreateLockFile();

            string[] levelsToBuild = allLevels ? GetLevelPaths() : GetLevelsToBuild();

            ConvertTileMaps();
            CreateAddressableSettings(!allTargets);
            AddressableAssetSettings.CleanPlayerContent();
            if (Directory.Exists(GetServerDataPath()) && (packageMode == 0 || allLevels)) Directory.Delete(GetServerDataPath(), true);

            // set build targets
            List<BuildTarget> targets = new List<BuildTarget>();
            // if (allTargets) targets.Add(BuildTarget.Android);
            targets.Add(BuildTarget.StandaloneWindows64); // set windows last so that we can continue with editor iterations normally right afterwards

            // build each level individually
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            foreach (string dir in levelsToBuild)
            {
                string levelName = Path.GetFileName(dir);

                string serverDir = GetServerDataPath() + "/Levels/" + Path.GetFileName(dir);
                if (packageMode == 1 && !allLevels && Directory.Exists(serverDir)) Directory.Delete(serverDir, true);

                settings.activeProfileId = settings.profileSettings.GetProfileId(levelName);
                settings.groups.ForEach(group =>
                {
                    if (group.ReadOnly) return;
                    group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = group.name == levelName;
                });

                BundledAssetGroupSchema schema = settings.groups.Where(group => group.name == levelName).First().GetSchema<BundledAssetGroupSchema>();
                settings.RemoteCatalogBuildPath = schema.BuildPath;
                settings.RemoteCatalogLoadPath = schema.LoadPath;

                // iterate over all supported platforms
                foreach (BuildTarget target in targets)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(target);
                    AddressableAssetSettings.BuildPlayerContent();
                }
            }

            RenameCatalogs();
            dirWatcher.affectedFiles.Clear(); // only do at end, since during build might cause false positives
            RemoveLockFile();

            Debug.Log("Packaging completed.");
        }

        private IEnumerator CreateDocumentation()
        {
            documentationInProgress = true;

            foreach (string dir in GetLevelPaths())
            {
                string levelName = Path.GetFileName(dir);
                string docuPath = $"{Application.dataPath}/../Documentation/{levelName}/";
                if (Directory.Exists(docuPath)) Directory.Delete(docuPath, true);
                Directory.CreateDirectory(docuPath);

                string root = $"Assets/Levels/{levelName}/";
                foreach (string folder in new[] { "Pieces", "Sceneries" })
                {
                    Directory.CreateDirectory(docuPath + folder);

                    string[] assets = AssetDatabase.FindAssets("*", new[] { $"{root}{folder}" });
                    foreach (string asset in assets)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                        if (!assetPath.ToLower().EndsWith(".prefab")) continue;

                        GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);

                        // this is asynchronous, and really shitty
                        // it will run into endless loops and other strange things on Unity side
                        // pounding it with a retry hammer seems to work though, just takes much longer
                        AssetPreview.GetAssetPreview(prefab);
                        Repaint(); // force async start
                        float startTime = Time.realtimeSinceStartup;
                        while (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
                        {
                            if (Time.realtimeSinceStartup > startTime + 2f)
                            {
                                AssetPreview.GetAssetPreview(prefab);
                                startTime = Time.realtimeSinceStartup;
                            }
                            yield return new WaitForSeconds(0.5f);
                        }

                        // still will not return something for all assets
                        Texture2D icon = AssetPreview.GetAssetPreview(prefab);
                        if (icon == null || !icon.isReadable) continue;

                        byte[] bytes = icon.EncodeToPNG();
                        if (bytes == null) continue;

                        File.WriteAllBytes(docuPath + folder + "/" + Path.GetFileName(assetPath) + ".png", bytes);
                    }
                }

                // fill HTML template
                string id = AssetDatabase.FindAssets("_LevelDocu")[0];
                string path = AssetDatabase.GUIDToAssetPath(id);
                DirectoryCopy(Application.dataPath + "/../" + path, docuPath);
                AssetDatabase.Refresh();

                string html = File.ReadAllText(docuPath + "level.html");
                html = html.Replace("{LevelName}", levelName);
                html = html.Replace("{LevelKey}", levelName);
                html = html.Replace("{AppVersion}", Application.version); // FIXME: points to wrong version
                html = html.Replace("{Date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                foreach (string folder in new[] { "Images", "Logic", "Materials", "Pieces", "Sceneries", "Audio/Effects", "Audio/Music" })
                {
                    string[] assets = new string[0];
                    string objects = "";

                    if (Directory.Exists($"{root}{folder}"))
                    {
                        Directory.CreateDirectory(docuPath + folder);

                        assets = AssetDatabase.FindAssets("*", new[] { $"{root}{folder}" });
                        foreach (string asset in assets)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(asset);

                            objects += "<div class=\"media\">";
                            string imageName = folder + "/" + Path.GetFileName(assetPath) + ".png";
                            if (File.Exists(docuPath + imageName))
                            {
                                objects += "<img src=\"" + imageName + "\" class=\"mr-3\">";
                            }
                            objects += "<div class=\"media-body\">/" + levelName + "/" + Path.GetFileNameWithoutExtension(assetPath);
                            objects += "</div></div>";
                        }
                        objects += "</table>";
                    }
                    html = html.Replace($"{{{Path.GetFileName(folder)}List}}", objects);
                    html = html.Replace($"{{{Path.GetFileName(folder)}Count}}", assets.Length.ToString());
                }

                File.WriteAllText(docuPath + "level.html", html);
                Help.BrowseURL(docuPath + "level.html");
            }
            documentationInProgress = false;
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName)) Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static string GetLockFileLocation()
        {
            return Application.dataPath + "/../" + LOCKFILE_NAME;
        }

        private void CreateLockFile()
        {
            packagingInProgress = true;
            if (!File.Exists(GetLockFileLocation())) File.Create(GetLockFileLocation());
        }

        private void RemoveLockFile()
        {
            if (File.Exists(GetLockFileLocation())) File.Delete(GetLockFileLocation());
            packagingInProgress = false;
        }

        private void ConvertTileMaps()
        {
            foreach (string extension in new[] { TileMapUtil.MAP_EXTENSION, TileMapUtil.WORLD_EXTENSION })
            {
                string[] files = Directory.GetFiles(Application.dataPath, "*." + extension, SearchOption.AllDirectories);
                TileMapUtil.ConvertTileMaps(files.ToList(), TravrsalSettingsManager.Get<string>("tiledPath"));
            }
            AssetDatabase.Refresh();
        }

        private void RenameCatalogs()
        {
            foreach (string path in new[] { GetServerDataPath(), Application.dataPath + "/../Library/com.unity.addressables/aa/Windows/Levels" })
            {
                if (Directory.Exists(path))
                {
                    foreach (string extension in new[] { "hash", "json" })
                    {
                        string[] files = Directory.GetFiles(path, "catalog_*." + extension, SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            File.Move(file, Path.GetDirectoryName(file) + "/catalog." + extension);
                        }
                    }
                }
            }
        }

        private void CreateAddressableSettings(bool localMode)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.ActivePlayModeDataBuilderIndex = 2;
            settings.BuildRemoteCatalog = true;
            settings.DisableCatalogUpdateOnStartup = true;

            // don't include built-in data, causes shader issues
            PlayerDataGroupSchema schema = settings.groups[0].GetSchema<PlayerDataGroupSchema>();
            schema.IncludeBuildSettingsScenes = false;
            schema.IncludeResourcesFolders = false;

            // setup profiles
            AddressableAssetProfileSettings profile = settings.profileSettings;

            // activate and (re)group assets
            foreach (string dir in GetLevelPaths())
            {
                string levelName = Path.GetFileName(dir);
                bool isBase = levelName == "Base";

                // create one profile per level
                string profileId = profile.GetProfileId(levelName);
                if (string.IsNullOrEmpty(profileId)) profileId = profile.AddProfile(levelName, settings.activeProfileId);

                string guid = AssetDatabase.AssetPathToGUID($"Assets/Levels/{levelName}");

                // create group if non-existent
                AddressableAssetGroup group = settings.groups.Where(g => g.name == levelName).FirstOrDefault();
                if (group == null) group = CreateAssetGroup<BundledAssetGroupSchema>(settings, levelName);

                // set correct path
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                entry.SetAddress($"Levels/{levelName}");

                // set variables
                string localRoot = Application.dataPath + $"/../Library/com.unity.addressables/aa/Windows/Levels/{levelName}/[BuildTarget]";
                profile.SetValue(profileId, AddressableAssetSettings.kLocalBuildPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kLocalLoadPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, $"ServerData/Levels/{levelName}/[BuildTarget]");
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, $"{AWSUtil.S3Root}Levels/{levelName}/[BuildTarget]");

                // ensure correct group settings
                BundledAssetGroupSchema groupSchema = group.GetSchema<BundledAssetGroupSchema>();
                groupSchema.UseAssetBundleCache = true;
                groupSchema.UseAssetBundleCrc = false;
                groupSchema.IncludeInBuild = isBase ? true : false;
                groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
                groupSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                groupSchema.BuildPath.SetVariableByName(settings, localMode ? AddressableAssetSettings.kLocalBuildPath : AddressableAssetSettings.kRemoteBuildPath);
                groupSchema.LoadPath.SetVariableByName(settings, localMode ? AddressableAssetSettings.kLocalLoadPath : AddressableAssetSettings.kRemoteLoadPath);
            }
        }

        private AddressableAssetGroup CreateAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
        {
            return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> { settings.DefaultGroup.Schemas[0] }, typeof(SchemaType));
        }

        private string[] GetLevelPaths()
        {
            return Directory.GetDirectories(Application.dataPath + "/Levels").Where(s => !Path.GetFileName(s).StartsWith("_")).ToArray();
        }

        private async void UploadLevels()
        {
            PackageLevels(true, true);

            if (!Directory.Exists(GetServerDataPath()))
            {
                Debug.LogError("Could not find directory to upload: " + GetServerDataPath());
                return;
            }

            uploadInProgress = true;
            uploadProgress = 0;
            uploadStartTime = DateTime.Now;

            AWSUtil aws = new AWSUtil();
            await aws.UploadDirectory(GetServerDataPath(), progress => uploadProgress = progress);

            EditorUtility.ClearProgressBar();
            uploadInProgress = false;

            Debug.Log("Upload completed.");
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}