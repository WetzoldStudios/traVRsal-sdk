using Asyncoroutine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace traVRsal.SDK
{
    public class PublishUI : BasicEditorUI
    {
        public const string LOCKFILE_NAME = "traVRsal.lock";

        private string[] PACKAGE_OPTIONS = { "Everything", "Intelligent" };

        private bool debugMode = false;
        private bool packagingInProgress = false;
        private bool documentationInProgress = false;
        private bool uploadInProgress = false;
        private bool verifyInProgress = false;
        private bool packagingSuccessful = false;
        private bool verificationPassed = false;
        private bool uploadPossible = false;

        private DateTime uploadStartTime;
        private float uploadProgress = 1;
        private int packageMode = 1;
        private static DirectoryWatcher dirWatcher;
        private static PublishUI window;
        private static Dictionary<string, VerificationResult> verifications = new Dictionary<string, VerificationResult>();

        [MenuItem("traVRsal/Publisher", priority = 110)]
        public static void ShowWindow()
        {
            window = GetWindow<PublishUI>("traVRsal Publisher");
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
            // independent of actual window
            if (uploadInProgress)
            {
                int timeRemaining = Mathf.Max(1, Mathf.RoundToInt((DateTime.Now.Subtract(uploadStartTime).Seconds / uploadProgress) * (1 - uploadProgress)));
                EditorUtility.DisplayProgressBar("Progress", "Uploading levels to server... " + timeRemaining + "s", uploadProgress);
            }
            base.OnGUI();

            GUILayout.Label("Packaging ensures that the editor shows the most up to date version of your level.", EditorStyles.wordWrappedLabel);

            // TODO: cache
            if (GetLevelPaths().Length > 1)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Packaging Mode:", EditorStyles.wordWrappedLabel);
                packageMode = GUILayout.SelectionGrid(packageMode, PACKAGE_OPTIONS, 2, EditorStyles.radioButton);
                GUILayout.EndHorizontal();
            }
            else
            {
                packageMode = 0;
            }
            EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress);
            string buttonText = "Package";
            if (packageMode == 1)
            {
                string[] levelsToBuild = GetLevelsToBuild().Select(path => Path.GetFileName(path)).ToArray();
                buttonText += " (" + ((dirWatcher.affectedFiles.Count > 0) ? string.Join(", ", levelsToBuild) : "everything") + ")";
            }

            if (GUILayout.Button(buttonText)) EditorCoroutineUtility.StartCoroutine(PackageLevels(packageMode == 2 ? true : false, packageMode == 2 ? true : false), this);
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || verifyInProgress);
            if (GUILayout.Button("Prepare Upload")) EditorCoroutineUtility.StartCoroutine(PrepareUpload(), this);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || !uploadPossible);
            if (GUILayout.Button("Upload")) EditorCoroutineUtility.StartCoroutine(UploadLevels(), this);
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();

            if (verifications.Count() > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Verification Results", EditorStyles.boldLabel);

                foreach (string dir in GetLevelPaths())
                {
                    string levelName = Path.GetFileName(dir);
                    if (!verifications.ContainsKey(levelName)) continue;

                    VerificationResult v = verifications[levelName];

                    v.showDetails = EditorGUILayout.Foldout(v.showDetails, levelName);

                    if (v.showDetails)
                    {
                        // PrintTableRow("Size (Original)", SDKUtil.BytesToString(v.sourceSize));
                        PrintTableRow("Size (Quest)", v.distroExistsAndroid ? SDKUtil.BytesToString(v.distroSizeAndroid) : "not packaged yet");
                        PrintTableRow("Size (PC)", v.distroExistsStandalone ? SDKUtil.BytesToString(v.distroSizeStandalone) : "not packaged yet");
                        if (v.documentationExists)
                        {
                            BeginPartialTableRow("Documentation");
                            if (GUILayout.Button("Open")) Help.BrowseURL(GetDocuPath(levelName) + "index.html");
                            EndPartialTableRow();
                        }
                        else
                        {
                            PrintTableRow("Documentation", "not created yet");
                        }
                    }
                }
            }

            if (debugMode)
            {
                EditorGUI.BeginDisabledGroup(documentationInProgress);
                if (GUILayout.Button("Create Documentation")) EditorCoroutineUtility.StartCoroutine(CreateDocumentation(), this);
                EditorGUI.EndDisabledGroup();
            }

            OnGUIDone();
        }

        private void PrintTableRow(string key, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(5));
            GUILayout.Label(key, GUILayout.Width(100));
            GUILayout.Label(value);
            GUILayout.EndHorizontal();
        }

        private void BeginPartialTableRow(string key)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(5));
            GUILayout.Label(key, GUILayout.Width(100));
        }

        private void EndPartialTableRow()
        {
            GUILayout.EndHorizontal();
        }

        private IEnumerator PrepareUpload()
        {
            yield return PackageLevels(true, true);
            yield return CreateDocumentation();
            PrepareCommonFiles();
            Verify();

            uploadPossible = packagingSuccessful && verificationPassed;
        }

        private void PrepareCommonFiles()
        {
            foreach (string dir in GetLevelPaths())
            {
                string levelName = Path.GetFileName(dir);
                string commonPath = GetServerDataPath() + "/Levels/" + levelName + "/Common/";
                string mediaPath = commonPath + "Media/";

                Directory.CreateDirectory(commonPath);

                // documentation
                File.Copy(GetDocuArchivePath(levelName), commonPath + "docs.zip", true);

                // images
                Directory.CreateDirectory(mediaPath);
                Level level = JsonConvert.DeserializeObject<Level>(File.ReadAllText(dir + "/level.json"));
                if (!string.IsNullOrEmpty(level.coverImage))
                {
                    File.Copy(dir + "/Images/" + level.coverImage, mediaPath + level.coverImage, true);
                }
            }
        }

        private void Verify()
        {
            verifyInProgress = true;
            verificationPassed = false;
            verifications.Clear();

            foreach (string dir in GetLevelPaths())
            {
                string levelName = Path.GetFileName(dir);

                VerificationResult result = new VerificationResult();
                result.sourceSize = DirectoryUtil.GetSize(dir);

                result.documentationPath = GetDocuArchivePath(levelName);
                result.documentationExists = File.Exists(result.documentationPath);

                result.distroPathAndroid = GetServerDataPath() + "/Levels/" + levelName + "/Android";
                result.distroExistsAndroid = Directory.Exists(result.distroPathAndroid);
                result.distroSizeAndroid = DirectoryUtil.GetSize(result.distroPathAndroid);

                result.distroPathStandalone = GetServerDataPath() + "/Levels/" + levelName + "/StandaloneWindows64";
                result.distroExistsStandalone = Directory.Exists(result.distroPathStandalone);
                result.distroSizeStandalone = DirectoryUtil.GetSize(result.distroPathStandalone);

                verifications.Add(levelName, result);
            }
            verifyInProgress = false;
            verificationPassed = true; // TODO: do some actual checks
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

        private IEnumerator PackageLevels(bool allLevels, bool allTargets)
        {
            uploadPossible = false;
            packagingInProgress = true;
            packagingSuccessful = false;

            try
            {
                string[] levelsToBuild = allLevels ? GetLevelPaths() : GetLevelsToBuild();
                if (levelsToBuild.Length == 0) yield break;

                CreateLockFile();
                ConvertTileMaps();
                CreateAddressableSettings(!allTargets);
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic; // FIXME: ASTC resulting in pink shaders as of 2019.4+
                EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;
                AddressableAssetSettings.CleanPlayerContent();
                if (Directory.Exists(GetServerDataPath()) && (packageMode == 0 || allLevels)) Directory.Delete(GetServerDataPath(), true);

                // set build targets
                List<BuildTarget> targets = new List<BuildTarget>();
                if (debugMode) // needed only due to strange Unity bug not allowing to automatically switch from PC to Android on some systems (reported)
                {
                    targets.Add(EditorUserBuildSettings.activeBuildTarget);
                }
                else
                {
                    targets.Add(BuildTarget.Android);
                    targets.Add(BuildTarget.StandaloneWindows64); // set windows last so that we can continue with editor iterations normally right afterwards
                }

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

                    if (allTargets)
                    {
                        // iterate over all supported platforms
                        foreach (BuildTarget target in targets)
                        {
                            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
                            AddressableAssetSettings.BuildPlayerContent();
                        }
                    }
                    else
                    {
                        // build only for currently active target
                        AddressableAssetSettings.BuildPlayerContent();
                    }
                }
                CreateAddressableSettings(!allTargets); // do again to have clean build state, as some settings were messed with while building
                RenameCatalogs();
                packagingSuccessful = true;
            }
            catch (Exception e)
            {
                packagingInProgress = false;
                EditorUtility.DisplayDialog("Error", "Packaging could not be completed. Error: " + e.Message, "Close");
                yield break;
            }

            dirWatcher.ClearAffected(); // only do at end, since during build might cause false positives
            RemoveLockFile();
            packagingInProgress = false;

            Debug.Log("Packaging completed successfully.");
        }

        private string GetDocuPath(string levelName)
        {
            return $"{Application.dataPath}/../Documentation/{levelName}/";
        }

        private string GetDocuArchivePath(string levelName)
        {
            return GetDocuPath(levelName) + "../" + levelName + "-docs.zip";
        }

        private IEnumerator CreateDocumentation()
        {
            documentationInProgress = true;

            string converterPath = TravrsalSettingsManager.Get("tiledPath", SDKUtil.TILED_PATH_DEFAULT);
            if (!string.IsNullOrEmpty(converterPath)) converterPath = Path.GetDirectoryName(converterPath) + "/tmxrasterizer.exe";

            foreach (string dir in GetLevelPaths())
            {
                string levelName = Path.GetFileName(dir);
                string docuPath = GetDocuPath(levelName);
                try
                {
                    if (Directory.Exists(docuPath)) Directory.Delete(docuPath, true);
                    Directory.CreateDirectory(docuPath);
                }
                catch
                {
                    EditorUtility.DisplayDialog("Error", $"Could not access documentation directory ({docuPath}). Most likely it is open somewhere in an explorer.", "OK");
                    continue;
                }

                string root = $"Assets/Levels/{levelName}/";

                // fill HTML template
                string id = AssetDatabase.FindAssets("_LevelDocu")[0];
                string path = AssetDatabase.GUIDToAssetPath(id);
                DirectoryUtil.Copy(Application.dataPath + "/../" + path, docuPath);
                AssetDatabase.Refresh();

                string html = File.ReadAllText(docuPath + "index.html");
                html = html.Replace("{LevelName}", levelName);
                html = html.Replace("{LevelKey}", levelName);
                html = html.Replace("{AppVersion}", Application.version); // FIXME: points to wrong version
                html = html.Replace("{Date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                foreach (string folder in new[] { "Data", "Images", "Logic", "Materials", "Pieces", "Sceneries", "Audio/Effects", "Audio/Music" })
                {
                    HashSet<string> doneAlready = new HashSet<string>();
                    string[] assets = new string[0];
                    string objects = "";
                    int objCount = 0;
                    string variableName = Path.GetFileName(folder);

                    if (Directory.Exists($"{root}{folder}"))
                    {
                        Directory.CreateDirectory(docuPath + folder);

                        assets = AssetDatabase.FindAssets("*", new[] { $"{root}{folder}" });
                        foreach (string asset in assets)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                            if (doneAlready.Contains(assetPath)) continue;
                            doneAlready.Add(assetPath);
                            if (assetPath.ToLower().EndsWith(".md")) continue;

                            string imageLink = "NoPreview.png";
                            bool withExtension = true;
                            bool generatePreview = false;
                            Type type = null;

                            switch (folder)
                            {
                                case "Data":
                                    if (!assetPath.ToLower().EndsWith(".tmx")) continue;
                                    imageLink = folder + "/" + Path.GetFileName(assetPath) + ".png";
                                    TileMapUtil.TileMapToImage(assetPath, docuPath + imageLink, converterPath);
                                    variableName = "Rooms";
                                    withExtension = false;
                                    break;

                                case "Images":
                                    generatePreview = true;
                                    type = typeof(Texture2D);
                                    break;

                                case "Materials":
                                    if (!assetPath.ToLower().EndsWith(".mat")) continue;
                                    withExtension = false;
                                    generatePreview = true;
                                    type = typeof(Material);
                                    break;

                                case "Pieces":
                                    if (!assetPath.ToLower().EndsWith(".prefab")) continue;
                                    withExtension = false;
                                    generatePreview = true;
                                    break;

                                case "Sceneries":
                                    if (!assetPath.ToLower().EndsWith(".prefab")) continue;
                                    withExtension = false;
                                    generatePreview = true;
                                    break;

                                case "Audio/Music":
                                case "Audio/Effects":
                                    generatePreview = true;
                                    type = typeof(AudioClip);
                                    break;
                            }
                            objCount++;

                            if (generatePreview)
                            {
                                GameObject prefab = null;
                                UnityEngine.Object obj = null;
                                if (type != null)
                                {
                                    obj = AssetDatabase.LoadAssetAtPath(assetPath, type);
                                }
                                else
                                {
                                    prefab = PrefabUtility.LoadPrefabContents(assetPath);
                                    obj = prefab;
                                }
                                if (obj != null)
                                {
                                    Texture2D icon = AssetPreview.GetAssetPreview(obj);

                                    while (icon == null && AssetPreview.IsLoadingAssetPreview(obj.GetInstanceID()))
                                    {
                                        Repaint();
                                        yield return new EditorWaitForSeconds(0.05f);
                                        icon = AssetPreview.GetAssetPreview(obj);
                                    }
                                    if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);

                                    // still will not return something for all assets
                                    if (icon == null || !icon.isReadable) continue;

                                    byte[] bytes = icon.EncodeToPNG();
                                    if (bytes == null) continue;

                                    File.WriteAllBytes(docuPath + folder + "/" + Path.GetFileName(assetPath) + ".png", bytes);
                                }
                                else
                                {
                                    // most likely a directory
                                    continue;
                                }
                            }

                            objects += "<div class=\"media mb-1\">";
                            string imageName = folder + "/" + Path.GetFileName(assetPath) + ".png";
                            string accessKey = assetPath.Substring((root + folder).Length + 1);
                            if (!withExtension) accessKey = accessKey.Substring(0, accessKey.LastIndexOf('.'));

                            if (generatePreview)
                            {
                                imageLink = File.Exists(docuPath + imageName) ? imageName : "MissingPreview.png";
                            }
                            objects += "<img src=\"" + imageLink + "\" class=\"mr-3\" width=\"128\">";
                            objects += "<div class=\"media-body\">/" + levelName + "/" + accessKey;
                            objects += "</div></div>";
                        }
                        objects += "</table>";
                    }
                    html = html.Replace($"{{{variableName}List}}", objects);
                    html = html.Replace($"{{{variableName}Count}}", objCount.ToString());
                }

                // copy data contents and level descriptor
                DirectoryUtil.Copy(root + "/Data", docuPath + "/Data");
                FileUtil.CopyFileOrDirectory(root + "/level.json", docuPath + "/level.json");

                // remove all meta files
                foreach (string fileName in Directory.EnumerateFiles(docuPath, "*.meta", SearchOption.AllDirectories))
                {
                    FileUtil.DeleteFileOrDirectory(fileName);
                }

                File.WriteAllText(docuPath + "index.html", html);

                // create zip
                if (File.Exists(GetDocuArchivePath(levelName))) File.Delete(GetDocuArchivePath(levelName));
                ZipFile.CreateFromDirectory(docuPath, GetDocuArchivePath(levelName), CompressionLevel.Fastest, false);
            }

            documentationInProgress = false;
        }

        public static string GetLockFileLocation()
        {
            return Application.dataPath + "/../" + LOCKFILE_NAME;
        }

        private void CreateLockFile()
        {
            if (!File.Exists(GetLockFileLocation())) File.Create(GetLockFileLocation());
        }

        private void RemoveLockFile()
        {
            if (File.Exists(GetLockFileLocation())) FileUtil.DeleteFileOrDirectory(GetLockFileLocation());
        }

        private void ConvertTileMaps()
        {
            foreach (string extension in new[] { TileMapUtil.MAP_EXTENSION, TileMapUtil.WORLD_EXTENSION })
            {
                string[] files = Directory.GetFiles(Application.dataPath, "*." + extension, SearchOption.AllDirectories);
                TileMapUtil.ConvertTileMaps(files.ToList(), TravrsalSettingsManager.Get("tiledPath", SDKUtil.TILED_PATH_DEFAULT));
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
                            string targetFile = Path.GetDirectoryName(file) + "/catalog." + extension;
                            if (File.Exists(targetFile)) FileUtil.DeleteFileOrDirectory(targetFile);
                            FileUtil.MoveFileOrDirectory(file, targetFile);
                        }
                    }
                }
            }
        }

        private void CreateAddressableSettings(bool localMode)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.ActivePlayModeDataBuilderIndex = localMode ? 0 : 2;
            settings.BuildRemoteCatalog = true;
            settings.DisableCatalogUpdateOnStartup = true;
            settings.ContiguousBundles = true;

            // don't include built-in data, causes shader issues
            settings.groups.ForEach(g =>
            {
                PlayerDataGroupSchema schema = g.GetSchema<PlayerDataGroupSchema>();
                if (schema != null)
                {
                    schema.IncludeBuildSettingsScenes = false;
                    schema.IncludeResourcesFolders = false;
                }
            });

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
                groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash; // hash to disimbiguate identically named files yields same error messages, e.g. standard shaders
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

        private IEnumerator UploadLevels()
        {
            if (!Directory.Exists(GetServerDataPath()))
            {
                Debug.LogError("Could not find directory to upload: " + GetServerDataPath());
                yield break;
            }

            uploadInProgress = true;
            uploadProgress = 0;
            uploadStartTime = DateTime.Now;

            AWSUtil aws = new AWSUtil();
            yield return aws.UploadDirectory(GetServerDataPath(), progress => uploadProgress = progress).AsCoroutine();

            EditorUtility.ClearProgressBar();
            uploadInProgress = false;

            EditorUtility.DisplayDialog("Success", "Upload completed.", "OK");
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}