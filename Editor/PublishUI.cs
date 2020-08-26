using Asyncoroutine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Networking;
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
        private bool worldListMismatch = false;

        private DateTime uploadStartTime;
        private float uploadProgress = 1;
        private int progressId;
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
            CreateDirWatcher();
        }

        private void CreateDirWatcher()
        {
            if (dirWatcher == null)
            {
                dirWatcher = new DirectoryWatcher(new FSWParams(GetWorldsRoot(false)));
                dirWatcher.StartFSW();
            }
        }

        public override void OnGUI()
        {
            // independent of actual window
            if (uploadInProgress)
            {
                int timeRemaining = Mathf.Max(1, Mathf.RoundToInt((DateTime.Now.Subtract(uploadStartTime).Seconds / uploadProgress) * (1 - uploadProgress)));
                Progress.Report(progressId, uploadProgress, "Uploading worlds to server... " + timeRemaining + "s");
            }
            base.OnGUI();

            GUILayout.Label("Packaging ensures that the editor shows the most up to date version of your world.", EditorStyles.wordWrappedLabel);

            // TODO: cache
            string[] worlds = GetWorldPaths();
            if (worlds.Length == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("There are no worlds created yet. Use the Setup tool to create one.", MessageType.Info);
            }
            else
            {
                if (worlds.Length > 1)
                {
                    EditorGUILayout.Space();
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
                    string[] worldsToBuild = GetWorldsToBuild().Select(path => Path.GetFileName(path)).ToArray();
                    buttonText += " (" + ((dirWatcher.affectedFiles.Count > 0) ? string.Join(", ", worldsToBuild) : "everything") + ")";
                }

                if (GUILayout.Button(buttonText)) EditorCoroutineUtility.StartCoroutine(PackageWorlds(packageMode == 2 ? true : false, packageMode == 2 ? true : false), this);
                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || verifyInProgress);
                if (GUILayout.Button("Prepare Upload")) EditorCoroutineUtility.StartCoroutine(PrepareUpload(), this);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || !uploadPossible);
                if (GUILayout.Button("Upload")) EditorCoroutineUtility.StartCoroutine(UploadWorlds(), this);
                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();

                CheckTokenGUI();
                if (worldListMismatch && !SDKUtil.networkIssue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The worlds inside your Worlds folder do not match your registered worlds on www.traVRsal.com. You probably need to rename these locally to match exactly.", MessageType.Error);
                    if (GUILayout.Button("Refresh")) EditorCoroutineUtility.StartCoroutine(RefreshVerify(), this);
                }

                if (verifications.Count() > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Verification Results", EditorStyles.boldLabel);

                    foreach (string dir in GetWorldPaths())
                    {
                        string worldName = Path.GetFileName(dir);
                        if (!verifications.ContainsKey(worldName)) continue;

                        VerificationResult v = verifications[worldName];

                        v.showDetails = EditorGUILayout.Foldout(v.showDetails, worldName);

                        if (v.showDetails)
                        {
                            PrintTableRow("Size (Quest)", v.distroExistsAndroid ? SDKUtil.BytesToString(v.distroSizeAndroid) : "not packaged yet");
                            PrintTableRow("Size (PC)", v.distroExistsStandalone ? SDKUtil.BytesToString(v.distroSizeStandalone) : "not packaged yet");
                            if (v.documentationExists)
                            {
                                BeginPartialTableRow("Documentation");
                                if (GUILayout.Button("Open")) Help.BrowseURL(GetDocuPath(worldName) + "index.html");
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
            }

            OnGUIDone();
        }

        private IEnumerator RefreshVerify()
        {
            yield return EditorCoroutineUtility.StartCoroutine(FetchUserWorlds(), this);
            Verify();
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
            yield return FetchUserWorlds();
            yield return PackageWorlds(true, true);
            yield return CreateDocumentation();
            PrepareCommonFiles();
            Verify();

            uploadPossible = packagingSuccessful && verificationPassed;
        }

        private void PrepareCommonFiles()
        {
            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);
                string commonPath = GetServerDataPath() + "/Worlds/" + worldName + "/Common/";
                string mediaPath = commonPath + "Media/";

                Directory.CreateDirectory(commonPath);

                // documentation
                File.Copy(GetDocuArchivePath(worldName), commonPath + "docs.zip", true);

                // images
                Directory.CreateDirectory(mediaPath);
                World world = SDKUtil.ReadJSONFileDirect<World>(dir + "/World.json");
                if (!string.IsNullOrEmpty(world.coverImage))
                {
                    File.Copy(dir + "/Images/" + world.coverImage, mediaPath + world.coverImage, true);
                }
            }
        }

        private void Verify()
        {
            verifyInProgress = true;
            verificationPassed = false;
            worldListMismatch = false;
            verifications.Clear();

            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);

                VerificationResult result = new VerificationResult();
                result.sourceSize = DirectoryUtil.GetSize(dir);

                result.documentationPath = GetDocuArchivePath(worldName);
                result.documentationExists = File.Exists(result.documentationPath);

                result.distroPathAndroid = GetServerDataPath() + "/Worlds/" + worldName + "/Android";
                result.distroExistsAndroid = Directory.Exists(result.distroPathAndroid);
                result.distroSizeAndroid = DirectoryUtil.GetSize(result.distroPathAndroid);

                result.distroPathStandalone = GetServerDataPath() + "/Worlds/" + worldName + "/StandaloneWindows64";
                result.distroExistsStandalone = Directory.Exists(result.distroPathStandalone);
                result.distroSizeStandalone = DirectoryUtil.GetSize(result.distroPathStandalone);

                verifications.Add(worldName, result);

                if (userWorlds != null && userWorlds.Where(w => w.key == worldName).Count() == 0)
                {
                    Debug.LogError("Found unregistered world: " + worldName);
                    worldListMismatch = true;
                }
            }
            verifyInProgress = false;
            verificationPassed = !worldListMismatch && !SDKUtil.invalidAPIToken && !SDKUtil.networkIssue; // TODO: do some actual checks
        }

        private string GetServerDataPath()
        {
            return Application.dataPath + "/../ServerData";
        }

        private string[] GetWorldsToBuild()
        {
            string[] worldsToBuild = GetWorldPaths();
            if (packageMode == 1)
            {
                try
                {
                    string[] filteredWorlds = worldsToBuild.Where(
                        worldName => dirWatcher.affectedFiles.Where(
                            af => af.Contains("/" + Path.GetFileName(worldName) + "/") || af.Contains("\\" + Path.GetFileName(worldName) + "\\")).Any()
                        ).ToArray();
                    if (filteredWorlds.Length > 0) worldsToBuild = filteredWorlds;
                }
                catch { }
            }

            return worldsToBuild;
        }

        private IEnumerator PackageWorlds(bool allWorlds, bool allTargets)
        {
            uploadPossible = false;
            packagingInProgress = true;
            packagingSuccessful = false;

            try
            {
                string[] worldsToBuild = allWorlds ? GetWorldPaths() : GetWorldsToBuild();
                if (worldsToBuild.Length == 0) yield break;

                CreateLockFile();
                ConvertTileMaps();
                CreateAddressableSettings(!allTargets);
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
                EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;
                AddressableAssetSettings.CleanPlayerContent();
                AssetDatabase.SaveAssets();

                if (Directory.Exists(GetServerDataPath()) && (packageMode == 0 || allWorlds)) Directory.Delete(GetServerDataPath(), true);

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

                // build each world individually
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                foreach (string dir in worldsToBuild)
                {
                    string worldName = Path.GetFileName(dir);
                    string serverDir = GetServerDataPath() + "/Worlds/" + Path.GetFileName(dir);

                    if (packageMode == 1 && !allWorlds && Directory.Exists(serverDir)) Directory.Delete(serverDir, true);

                    settings.activeProfileId = settings.profileSettings.GetProfileId(worldName);
                    settings.groups.ForEach(group =>
                    {
                        if (group.ReadOnly) return;
                        group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = group.name == worldName;
                    });

                    BundledAssetGroupSchema schema = settings.groups.Where(group => group.name == worldName).First().GetSchema<BundledAssetGroupSchema>();
                    settings.RemoteCatalogBuildPath = schema.BuildPath;
                    settings.RemoteCatalogLoadPath = schema.LoadPath;

                    CreateObjectLib(worldName);

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
                        // FIXME: will delete folder now in new addressables version so we need to save it out and restore later
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

            if (dirWatcher != null)
            {
                dirWatcher.ClearAffected(); // only do at end, since during build might cause false positives
            }
            else
            {
                CreateDirWatcher(); // can happen after initial project creation
            }
            RemoveLockFile();
            packagingInProgress = false;

            Debug.Log("Packaging completed successfully.");
        }

        private string GetDocuPath(string worldName)
        {
            return $"{Application.dataPath}/../Documentation/{worldName}/";
        }

        private void CreateObjectLib(string worldName)
        {
            string root = GetWorldsRoot(true) + "/" + worldName + "/";
            World world = SDKUtil.ReadJSONFileDirect<World>(root + "World.json");

            world.objectSpecs = new List<ObjectSpec>();
            string[] assets = AssetDatabase.FindAssets("*", new[] { root + "Pieces" });
            foreach (string asset in assets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                if (!assetPath.ToLower().EndsWith(".prefab")) continue;

                GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
                if (prefab.TryGetComponent(out ExtendedAttributes ea))
                {
                    if (!ea.spec.IsDefault())
                    {
                        ea.spec.objectKey = Path.GetFileNameWithoutExtension(assetPath);
                        world.objectSpecs.Add(ea.spec);
                    }
                }
                if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);
            }

            // write back
            world.NullifyEmpties();
            File.WriteAllText(root + "World.json", SDKUtil.SerializeObject(world));
        }

        private string GetDocuArchivePath(string worldName)
        {
            return GetDocuPath(worldName) + "../" + worldName + "-docs.zip";
        }

        private IEnumerator CreateDocumentation()
        {
            documentationInProgress = true;

            string converterPath = TravrsalSettingsManager.Get("tiledPath", SDKUtil.TILED_PATH_DEFAULT);
            if (!string.IsNullOrEmpty(converterPath)) converterPath = Path.GetDirectoryName(converterPath) + "/tmxrasterizer.exe";

            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);
                string docuPath = GetDocuPath(worldName);
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

                string root = GetWorldsRoot(true) + $"/{worldName}/";

                // fill HTML template
                string id = AssetDatabase.FindAssets("_WorldDocu")[0];
                string path = AssetDatabase.GUIDToAssetPath(id);
                DirectoryUtil.Copy(Application.dataPath + "/../" + path, docuPath);
                AssetDatabase.Refresh();

                string html = File.ReadAllText(docuPath + "index.html");
                html = html.Replace("{WorldName}", worldName);
                html = html.Replace("{WorldKey}", worldName);
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
                                    variableName = "Zones";
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
                            objects += "<div class=\"media-body\">/" + worldName + "/" + accessKey;
                            objects += "</div></div>";
                        }
                        objects += "</table>";
                    }
                    html = html.Replace($"{{{variableName}List}}", objects);
                    html = html.Replace($"{{{variableName}Count}}", objCount.ToString());
                }

                // copy data contents and world descriptor
                DirectoryUtil.Copy(root + "/Data", docuPath + "/Data");
                FileUtil.CopyFileOrDirectory(root + "/World.json", docuPath + "/World.json");

                // remove all meta files
                foreach (string fileName in Directory.EnumerateFiles(docuPath, "*.meta", SearchOption.AllDirectories))
                {
                    FileUtil.DeleteFileOrDirectory(fileName);
                }

                File.WriteAllText(docuPath + "index.html", html);

                // create zip
                if (File.Exists(GetDocuArchivePath(worldName))) File.Delete(GetDocuArchivePath(worldName));
                ZipFile.CreateFromDirectory(docuPath, GetDocuArchivePath(worldName), CompressionLevel.Fastest, false);
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
            foreach (string path in new[] { GetServerDataPath(), Application.dataPath + "/../Library/com.unity.addressables/aa/Windows/Worlds" })
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
            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);
                bool isBase = worldName == "Base";

                // create one profile per world
                string profileId = profile.GetProfileId(worldName);
                if (string.IsNullOrEmpty(profileId)) profileId = profile.AddProfile(worldName, settings.activeProfileId);

                string guid = AssetDatabase.AssetPathToGUID($"Assets/Worlds/{worldName}");

                // create group if non-existent
                AddressableAssetGroup group = settings.groups.Where(g => g.name == worldName).FirstOrDefault();
                if (group == null) group = CreateAssetGroup<BundledAssetGroupSchema>(settings, worldName);
                if (group.CanBeSetAsDefault()) settings.DefaultGroup = group; // default group ensures there is no accidental local default group resulting in local paths being baked into addressable for shaders

                // set correct path
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                entry.SetAddress($"Worlds/{worldName}");

                // set variables
                string localRoot = Application.dataPath + $"/../Library/com.unity.addressables/aa/Windows/Worlds/{worldName}/[BuildTarget]";
                profile.SetValue(profileId, AddressableAssetSettings.kLocalBuildPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kLocalLoadPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, $"ServerData/Worlds/{worldName}/[BuildTarget]");
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, $"{AWSUtil.S3Root}Worlds/{worldName}/[BuildTarget]");

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

        private IEnumerator UploadWorlds()
        {
            if (!Directory.Exists(GetServerDataPath()))
            {
                Debug.LogError("Could not find directory to upload: " + GetServerDataPath());
                yield break;
            }

            uploadInProgress = true;
            uploadProgress = 0;
            uploadStartTime = DateTime.Now;
            progressId = Progress.Start("Uploading worlds");

            AWSUtil aws = new AWSUtil();
            yield return aws.UploadDirectory(GetServerDataPath(), progress => uploadProgress = progress).AsCoroutine();
            yield return PublishWorldUpdates();

            Progress.Remove(progressId);
            uploadInProgress = false;

            EditorUtility.DisplayDialog("Success", "Upload completed.", "OK");
        }

        private IEnumerator PublishWorldUpdates()
        {
            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);
                string uri = SDKUtil.API_ENDPOINT + "userworlds/" + userWorlds.Where(w => w.key == worldName).First().id;

                // extract data from world descriptor
                string worldJson = File.ReadAllText(dir + "/World.json");
                World world = JsonConvert.DeserializeObject<World>(worldJson);

                UserWorld userWorld = new UserWorld();
                userWorld.cover_image = world.coverImage;
                userWorld.world_json = worldJson;
                userWorld.unity_version = Application.unityVersion;
                userWorld.is_virtual = world.isVirtual ? "1" : "0";
                userWorld.android_size = verifications[worldName].distroSizeAndroid;
                userWorld.pc_size = verifications[worldName].distroSizeStandalone;

                // TODO: convert to SDKUtil function as well
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userWorld));
                using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, data))
                {
                    webRequest.SetRequestHeader("Accept", "application/json");
                    webRequest.SetRequestHeader("Authorization", "Bearer " + GetAPIToken());
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError($"Could not update world {worldName} due to network issues: {webRequest.error}");
                    }
                    else if (webRequest.isHttpError)
                    {
                        if (webRequest.responseCode == (int)HttpStatusCode.Unauthorized)
                        {
                            SDKUtil.invalidAPIToken = true;
                            Debug.LogError("Invalid or expired API Token.");
                        }
                        else
                        {
                            Debug.LogError($"There was an error updating world {worldName}: {webRequest.error}");
                        }
                    }
                }
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}