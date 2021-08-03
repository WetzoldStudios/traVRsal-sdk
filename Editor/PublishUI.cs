using Asyncoroutine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Networking;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace traVRsal.SDK
{
    public class PublishUI : BasicEditorUI
    {
        private const string TTS_LANGUAGE_CODE = "en-US";
        private const string TTS_VOICE = "en-US-GuyNeural";
        private const string TTS_MOOD = "cheerful";
        private const string TTS_PITCH = "low";
        private const string TTS_SPEED = "";

        private const bool linuxSupport = true;

        private readonly string[] PACKAGE_OPTIONS = {"Everything", "Intelligent"};
        private readonly string[] RELEASE_CHANNELS = {"Production", "Beta", "Alpha"};

        private static bool debugMode = false;

        private static bool packagingInProgress;
        private static bool documentationInProgress;
        private static bool uploadErrors;
        private static bool speechErrors;
        private static bool uploadInProgress;
        private static bool verifyInProgress;
        private static bool packagingSuccessful;
        private static bool verificationPassed;
        private static bool uploadPossible;
        private static bool worldListMismatch;

        private int releaseChannel;
        private int packageMode = 1;

        private static DateTime uploadStartTime;
        private static float uploadProgress = 1;
        private static int uploadProgressId;
        private static int uncompressedTextures;
        private static DirectoryWatcher dirWatcher;
        private static PublishUI window;
        private static Dictionary<string, VerificationResult> verifications = new Dictionary<string, VerificationResult>();
        private static int preparedReleaseChannel;

        [MenuItem("traVRsal/Publisher", priority = 120)]
        public static void ShowWindow()
        {
            window = GetWindow<PublishUI>("traVRsal Publisher");
        }

        public override void OnEnable()
        {
            base.OnEnable();
            CreateDirWatcher();
        }

        private static void CreateDirWatcher()
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
                Progress.SetRemainingTime(uploadProgressId, timeRemaining);
                Progress.Report(uploadProgressId, uploadProgress, "Uploading worlds to server...");
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
                bool androidSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android);
                bool pcSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                bool linuxSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);

                if (!pcSupported && !linuxSupported)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Windows (IL2CPP) module not installed with Unity. Add this through the hub installer to be able to test in the studio.", MessageType.Error);
                    EditorGUILayout.Space();
                }
                else
                {
                    if (worlds.Length > 1)
                    {
                        EditorGUILayout.Space();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Packaging Mode:", EditorStyles.wordWrappedLabel);
                        packageMode = EditorGUILayout.Popup(packageMode, PACKAGE_OPTIONS);
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
                        string[] worldsToBuild = GetWorldsToBuild(packageMode).Select(Path.GetFileName).ToArray();
                        buttonText += " (" + ((dirWatcher.affectedFiles.Count > 0) ? string.Join(", ", worldsToBuild) : "everything") + ")";
                    }

                    if (GUILayout.Button(buttonText)) EditorCoroutineUtility.StartCoroutine(PackageWorlds(packageMode, releaseChannel, packageMode == 2, packageMode == 2), this);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.Space();
                GUILayout.Label("To test inside the Quest or to make the world accessible to others, upload it to the traVRsal server.", EditorStyles.wordWrappedLabel);

                if (!androidSupported)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Android module not installed with Unity. Add this through the hub installer to be able to deploy to Quest.", MessageType.Error);
                    EditorGUILayout.Space();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Release Channel:", EditorStyles.wordWrappedLabel);
                    releaseChannel = EditorGUILayout.Popup(releaseChannel, RELEASE_CHANNELS);

                    EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || verifyInProgress || documentationInProgress);
                    if (GUILayout.Button("Prepare Upload")) EditorCoroutineUtility.StartCoroutine(PrepareUpload(), this);
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }

                CheckTokenGUI();
                if (worldListMismatch && !SDKUtil.networkIssue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The worlds inside your Worlds folder do not match your registered worlds on www.traVRsal.com. You probably need to rename these locally to match exactly.", MessageType.Error);
                    if (GUILayout.Button("Refresh")) EditorCoroutineUtility.StartCoroutine(RefreshVerify(), this);
                }

                if (uncompressedTextures > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Uncompressed Textures in Project: " + uncompressedTextures, MessageType.Warning);
                    if (GUILayout.Button("Fix")) EditorCoroutineUtility.StartCoroutine(CompressTextures(), this);
                    GUILayout.EndHorizontal();
                }

                if (verifications.Count > 0)
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
                            PrintTableRow("Size (Windows)", v.distroExistsStandaloneWin ? SDKUtil.BytesToString(v.distroSizeStandaloneWin) : "not packaged yet");
                            if (linuxSupport) PrintTableRow("Size (Linux)", v.distroExistsStandaloneLinux ? SDKUtil.BytesToString(v.distroSizeStandaloneLinux) : "not packaged yet");
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
                            BeginPartialTableRow("Actions");
                            EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || !uploadPossible);
                            if (GUILayout.Button("Upload")) EditorCoroutineUtility.StartCoroutine(UploadWorld(worldName), this);
                            if (debugMode && GUILayout.Button("Register")) EditorCoroutineUtility.StartCoroutine(PublishWorldUpdates(worldName), this);
                            EditorGUI.EndDisabledGroup();
                            EndPartialTableRow();
                        }
                    }
                }

                if (debugMode)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Debug mode is enabled.", MessageType.Warning);

                    EditorGUI.BeginDisabledGroup(packagingInProgress || uploadInProgress || verifyInProgress || documentationInProgress);
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

        private IEnumerator FetchTTS()
        {
            foreach (string dir in GetWorldPaths())
            {
                World world = SDKUtil.ReadJSONFileDirect<World>(dir + "/World.json");
                if (world.isVirtual) continue;

                string voicePath = dir + "/Audio/Voice/";
                Directory.CreateDirectory(voicePath);

                // generate loading text
                // TODO: hash & cache + support custom loading text
                string text = world.journeys?.Count > 0 ? $"Creating a random and unique play-through for {world.name}. " : $"Loading {world.name}. ";
                text += string.IsNullOrEmpty(world.longDescription) ? world.shortDescription : world.longDescription;

                string targetFile = voicePath + SDKUtil.VOICE_LOADING_WORLD;
                bool successful = false;
                if (File.Exists(targetFile)) File.Delete(targetFile);
                yield return FetchSpeech(text, targetFile, result => successful = result);
                if (!successful) yield break;
            }
        }

        private IEnumerator PrepareUpload(bool force = false, bool linuxOnly = false)
        {
            preparedReleaseChannel = releaseChannel;

            yield return FetchUserWorlds();
            yield return FetchTTS();
            yield return PackageWorlds(packageMode, releaseChannel, true, true, force, linuxOnly);
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
                if (world.chapters != null)
                {
                    foreach (Chapter chapter in world.chapters)
                    {
                        if (!string.IsNullOrEmpty(chapter.coverImage))
                        {
                            File.Copy(dir + "/Images/" + chapter.coverImage, mediaPath + chapter.coverImage, true);
                        }
                    }
                }
            }
        }

        private IEnumerable<TextureImporter> GetUncompressedTextures()
        {
            return AssetDatabase.FindAssets("t:texture", null)
                .Select(guid => AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as TextureImporter)
                .Where(ti => ti != null)
                .Where(ti => !ti.assetPath.StartsWith("Packages/"))
                .Where(ti => !ti.assetPath.Contains("/Editor/"))
                .Where(ti => ti.textureCompression == TextureImporterCompression.Uncompressed || !ti.crunchedCompression);
        }

        private IEnumerator CompressTextures()
        {
            IEnumerable<TextureImporter> uncompressed = GetUncompressedTextures();

            int progressId = Progress.Start("Compressing project textures");
            int current = 0;
            int total = uncompressed.Count();
            foreach (TextureImporter textureImporter in uncompressed)
            {
                current++;
                Progress.Report(progressId, (float) current / total, textureImporter.assetPath);

                if (textureImporter.textureCompression == TextureImporterCompression.Uncompressed) textureImporter.textureCompression = TextureImporterCompression.Compressed;
                textureImporter.crunchedCompression = true;
                textureImporter.compressionQuality = 50;
                AssetDatabase.ImportAsset(textureImporter.assetPath);

                yield return null;
            }
            uncompressedTextures = 0;

            Progress.Remove(progressId);
            EditorUtility.DisplayDialog("Done", "Texture compression completed. Prepare the upload again.", "OK");
        }

        private void Verify()
        {
            verifyInProgress = true;
            verificationPassed = false;
            worldListMismatch = false;
            verifications.Clear();

            IEnumerable<TextureImporter> uncompressed = GetUncompressedTextures();
            uncompressedTextures = uncompressed.Count();

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

                result.distroPathStandaloneWin = GetServerDataPath() + "/Worlds/" + worldName + "/StandaloneWindows64";
                result.distroExistsStandaloneWin = Directory.Exists(result.distroPathStandaloneWin);
                result.distroSizeStandaloneWin = DirectoryUtil.GetSize(result.distroPathStandaloneWin);

                result.distroPathStandaloneLinux = GetServerDataPath() + "/Worlds/" + worldName + "/StandaloneLinux64";
                result.distroExistsStandaloneLinux = Directory.Exists(result.distroPathStandaloneLinux);
                result.distroSizeStandaloneLinux = DirectoryUtil.GetSize(result.distroPathStandaloneLinux);

                verifications.Add(worldName, result);

                if (userWorlds != null && userWorlds.Count(w => w.key == worldName) == 0)
                {
                    Debug.LogError($"Found unregistered world: {worldName}");
                    worldListMismatch = true;
                }
            }

            verifyInProgress = false;
            verificationPassed = !worldListMismatch && !SDKUtil.invalidAPIToken && !SDKUtil.networkIssue; // TODO: do some actual checks
        }

        private static string GetServerDataPath()
        {
            return Application.dataPath + "/../ServerData";
        }

        private static string[] GetWorldsToBuild(int packageMode)
        {
            string[] worldsToBuild = GetWorldPaths();
            if (packageMode == 1)
            {
                try
                {
                    string[] filteredWorlds = worldsToBuild.Where(
                        worldName => dirWatcher.affectedFiles.Any(af => af.Contains("/" + Path.GetFileName(worldName) + "/") || af.Contains("\\" + Path.GetFileName(worldName) + "\\"))
                    ).ToArray();
                    if (filteredWorlds.Length > 0) worldsToBuild = filteredWorlds;
                }
                catch
                {
                }
            }

            return worldsToBuild;
        }

        public static IEnumerator PackageWorlds(int packageMode, int releaseChannel, bool allWorlds, bool allTargets, bool force = false, bool linuxOnly = false)
        {
            uploadPossible = false;
            packagingInProgress = true;
            packagingSuccessful = false;

            try
            {
                string[] worldsToBuild = allWorlds ? GetWorldPaths() : GetWorldsToBuild(packageMode);
                if (worldsToBuild.Length == 0) yield break;
                string resultFolder = Application.dataPath + "/../traVRsal/";
                BuildTarget mainTarget = linuxOnly || Application.platform == RuntimePlatform.LinuxEditor ? BuildTarget.StandaloneLinux64 : BuildTarget.StandaloneWindows64;

                CreateLockFile();
                ConvertTileMaps();
                CreateAddressableSettings(!allTargets, releaseChannel);
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
                EditorUserBuildSettings.selectedStandaloneTarget = mainTarget;
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x); // Linux can only be built with Mono on Windows

                AddressableAssetSettings.CleanPlayerContent();
                AssetDatabase.SaveAssets();

                if (Directory.Exists(GetServerDataPath()) && (packageMode == 0 || allWorlds)) Directory.Delete(GetServerDataPath(), true);

                // set build targets
                List<Tuple<BuildTargetGroup, BuildTarget>> targets = new List<Tuple<BuildTargetGroup, BuildTarget>>();
                if (allTargets)
                {
                    targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Android, BuildTarget.Android));

                    // set windows/linux last so that we can continue with editor iterations normally right afterwards
                    if (Application.platform == RuntimePlatform.LinuxEditor)
                    {
                        targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
                        targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64));
                    }
                    else
                    {
                        if (linuxSupport) targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64));
                        targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
                    }
                }
                else
                {
                    targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, mainTarget));
                }

                // update world content
                foreach (string dir in worldsToBuild)
                {
                    string worldName = Path.GetFileName(dir);
                    if (!UpdateWorldData(worldName)) yield break;
                }

                // iterate over all supported platforms
                foreach (Tuple<BuildTargetGroup, BuildTarget> target in targets)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(target.Item1, target.Item2);

                    // build each world individually
                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                    foreach (string dir in worldsToBuild)
                    {
                        string worldName = Path.GetFileName(dir);
                        string serverDir = GetServerDataPath() + "/Worlds/" + Path.GetFileName(dir);
                        if (!allTargets && Directory.Exists(resultFolder + worldName)) Directory.Delete(resultFolder + worldName, true);

                        if (packageMode == 1 && !allWorlds && Directory.Exists(serverDir)) Directory.Delete(serverDir, true);

                        settings.activeProfileId = settings.profileSettings.GetProfileId(worldName);
                        settings.groups.ForEach(group =>
                        {
                            if (group.ReadOnly) return;
                            group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = group.name == worldName;

                            // default group ensures there is no accidental local default group resulting in local paths being baked into addressable for shaders
                            if (group.name == worldName && group.CanBeSetAsDefault()) settings.DefaultGroup = group;
                        });

                        BundledAssetGroupSchema schema = settings.groups.First(group => @group.name == worldName).GetSchema<BundledAssetGroupSchema>();
                        settings.RemoteCatalogBuildPath = schema.BuildPath;
                        settings.RemoteCatalogLoadPath = schema.LoadPath;
                        settings.ShaderBundleCustomNaming = worldName;

                        AddressableAssetSettings.BuildPlayerContent();
                    }
                }
                CreateAddressableSettings(!allTargets, releaseChannel); // do again to have clean build state, as some settings were messed with while building
                RenameCatalogs();
                packagingSuccessful = true;
            }
            catch (Exception e)
            {
                packagingInProgress = false;
                EditorUtility.DisplayDialog("Error", $"Packaging could not be completed. Error: {e.Message}", "Close");
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

        private static bool UpdateWorldData(string worldName)
        {
            string root = GetWorldsRoot() + "/" + worldName + "/";
            World world = SDKUtil.ReadJSONFileDirect<World>(root + "World.json");
            if (world == null)
            {
                EditorUtility.DisplayDialog("Error", $"World.json file for {worldName} seems corrupted and needs to be fixed first.", "OK");
                return false;
            }

            string worldBasePath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(AssetDatabase.AssetPathToGUID(root + "World.json"))) + "/Pieces";
            world.objectSpecs = new List<ObjectSpec>();
            world.usedTags = new HashSet<string>();
            string[] assets = AssetDatabase.FindAssets("*", new[] {root + "Pieces"});
            foreach (string asset in assets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                if (!assetPath.ToLower().EndsWith(".prefab")) continue;

                GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
                if (!string.IsNullOrWhiteSpace(prefab.tag) && !prefab.CompareTag("Untagged")) world.usedTags.Add(prefab.tag);

                if (prefab.TryGetComponent(out ExtendedAttributes ea))
                {
                    if (!ea.spec.IsDefault())
                    {
                        string prefix = Path.GetDirectoryName(assetPath);
                        if (prefix != null && prefix.Length > worldBasePath.Length)
                        {
                            prefix = prefix.Substring(worldBasePath.Length + 1) + "/";
                            prefix = prefix.Replace('\\', '/');
                        }
                        else
                        {
                            prefix = "";
                        }

                        string fileName = Path.GetFileNameWithoutExtension(assetPath);
                        ea.spec.objectKey = prefix + fileName;
                        world.objectSpecs.Add(ea.spec);
                    }
                }
                if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);
            }

            // increase version
            if (string.IsNullOrEmpty(world.version))
            {
                world.version = "0.0.1";
            }
            else
            {
                if (Version.TryParse(world.version, out Version version))
                {
                    world.version = $"{version.Major}.{version.Minor}.{version.Build + 1}";
                }
                else
                {
                    world.version = "0.0.1";
                    Debug.LogError($"World.json of {worldName} contained an unreadable version. Resetting to {world.version}.");
                }
            }
            world.versionCode++;

            // write back
            world.NullifyEmpties();
            File.WriteAllText(root + "World.json", SDKUtil.SerializeObject(world, DefaultValueHandling.Ignore));

            return true;
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

                string root = GetWorldsRoot() + $"/{worldName}/";

                // fill HTML template
                string id = AssetDatabase.FindAssets("WorldDocu")[0];
                string path = AssetDatabase.GUIDToAssetPath(id);
                DirectoryUtil.Copy(Application.dataPath + "/../" + path, docuPath);
                AssetDatabase.Refresh();

                string html = File.ReadAllText(docuPath + "index.html");
                html = html.Replace("{WorldName}", worldName);
                html = html.Replace("{WorldKey}", worldName);
                html = html.Replace("{AppVersion}", Application.version); // FIXME: points to wrong version
                html = html.Replace("{Date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                foreach (string folder in new[] {"Data", "Images", "Logic", "Materials", "Pieces", "Sceneries", "Audio/Effects", "Audio/Music"})
                {
                    HashSet<string> doneAlready = new HashSet<string>();
                    string[] assets = new string[0];
                    string objects = "";
                    int objCount = 0;
                    string variableName = Path.GetFileName(folder);
                    switch (folder)
                    {
                        case "Data":
                            variableName = "Zones";
                            break;
                    }
                    if (Directory.Exists($"{root}{folder}"))
                    {
                        Directory.CreateDirectory(docuPath + folder);

                        assets = AssetDatabase.FindAssets("*", new[] {$"{root}{folder}"});
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
                                UnityEngine.Object obj;
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
            return Application.dataPath + "/../" + SDKUtil.LOCKFILE_NAME;
        }

        private static void CreateLockFile()
        {
            if (!File.Exists(GetLockFileLocation())) File.Create(GetLockFileLocation());
        }

        private static void RemoveLockFile()
        {
            if (File.Exists(GetLockFileLocation())) FileUtil.DeleteFileOrDirectory(GetLockFileLocation());
        }

        private static void ConvertTileMaps()
        {
            foreach (string extension in new[] {TileMapUtil.MAP_EXTENSION, TileMapUtil.WORLD_EXTENSION})
            {
                string[] files = Directory.GetFiles(Application.dataPath, "*." + extension, SearchOption.AllDirectories);
                TileMapUtil.ConvertTileMaps(files.ToList(), TravrsalSettingsManager.Get("tiledPath", SDKUtil.TILED_PATH_DEFAULT));
            }
            AssetDatabase.Refresh();
        }

        private static void RenameCatalogs()
        {
            foreach (string path in new[] {GetServerDataPath(), Application.dataPath + "/../traVRsal"})
            {
                if (Directory.Exists(path))
                {
                    foreach (string extension in new[] {"hash", "json"})
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

        private static void CreateAddressableSettings(bool localMode, int releaseChannel)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.ActivePlayModeDataBuilderIndex = localMode ? 0 : 2;
            settings.BuildRemoteCatalog = true;
            settings.DisableCatalogUpdateOnStartup = true;
            settings.ContiguousBundles = true;
            settings.IgnoreUnsupportedFilesInBuild = true;
            settings.ShaderBundleNaming = ShaderBundleNaming.Custom;

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
                AddressableAssetGroup group = settings.groups.FirstOrDefault(g => g.name == worldName);
                if (group == null) group = CreateAssetGroup<BundledAssetGroupSchema>(settings, worldName);
                if (group.CanBeSetAsDefault()) settings.DefaultGroup = group; // default group ensures there is no accidental local default group resulting in local paths being baked into addressable for shaders

                // set correct path
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                entry.SetAddress($"Worlds/{worldName}");

                // set variables
                string localRoot = Application.dataPath + $"/../traVRsal/{worldName}/[BuildTarget]";
                string remoteTarget = null;
                switch (releaseChannel)
                {
                    case 0:
                        remoteTarget = AWSUtil.S3CDNRoot_Live;
                        break;

                    case 1:
                        remoteTarget = AWSUtil.S3CDNRoot_Beta;
                        break;

                    case 2:
                        remoteTarget = AWSUtil.S3CDNRoot_Alpha;
                        break;
                }
                profile.SetValue(profileId, AddressableAssetSettings.kLocalBuildPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kLocalLoadPath, localRoot);
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, $"ServerData/Worlds/{worldName}/[BuildTarget]");
                profile.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, $"{remoteTarget}Worlds/{worldName}/[BuildTarget]");

                // ensure correct group settings
                BundledAssetGroupSchema groupSchema = group.GetSchema<BundledAssetGroupSchema>();
                groupSchema.UseAssetBundleCache = true;
                groupSchema.UseAssetBundleCrc = false;
                groupSchema.IncludeInBuild = isBase;
                groupSchema.RetryCount = 3;
                groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash; // hash to disambiguate identically named files yields same error messages, e.g. standard shaders
                groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
                groupSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                groupSchema.BuildPath.SetVariableByName(settings, localMode ? AddressableAssetSettings.kLocalBuildPath : AddressableAssetSettings.kRemoteBuildPath);
                groupSchema.LoadPath.SetVariableByName(settings, localMode ? AddressableAssetSettings.kLocalLoadPath : AddressableAssetSettings.kRemoteLoadPath);
            }
        }

        private static AddressableAssetGroup CreateAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
        {
            return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> {settings.DefaultGroup.Schemas[0]}, typeof(SchemaType));
        }

        private IEnumerator UploadWorld(string worldName)
        {
            string path = GetServerDataPath() + "/Worlds/" + worldName;
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Could not find directory to upload: {path}");
                yield break;
            }

            uploadErrors = false;
            uploadInProgress = true;
            uploadProgress = 0;
            uploadStartTime = DateTime.Now;
            uploadProgressId = Progress.Start("Uploading world");

            AWSUtil aws = new AWSUtil();
            yield return aws.UploadDirectory(GetServerDataPath(), progress => uploadProgress = progress, "Worlds/" + worldName + "/*").AsCoroutine();
            yield return PublishWorldUpdates(worldName);

            Progress.Remove(uploadProgressId);
            uploadInProgress = false;

            if (uploadErrors)
            {
                EditorUtility.DisplayDialog("Error", "Upload could not be completed due to errors. Check the console for details.", "OK");
            }
            else
            {
                string channelName = null;
                switch (preparedReleaseChannel)
                {
                    case 0:
                        channelName = "PRODUCTION";
                        break;

                    case 1:
                        channelName = "BETA";
                        break;

                    case 2:
                        channelName = "ALPHA";
                        break;
                }
                EditorUtility.DisplayDialog("Success", $"Upload of {worldName} completed. Use the " + channelName + " app to test.", "OK");
            }
        }

        private IEnumerator PublishWorldUpdates(string worldName)
        {
            string channelName = null;
            switch (preparedReleaseChannel)
            {
                case 0:
                    channelName = "live";
                    break;

                case 1:
                    channelName = "beta";
                    break;

                case 2:
                    channelName = "alpha";
                    break;
            }

            string uri = SDKUtil.API_ENDPOINT + "userworlds/" + userWorlds.First(w => w.key == worldName).id + "?channel=" + channelName;

            // extract data from world descriptor
            string worldJson = File.ReadAllText(GetWorldsRoot() + "/" + worldName + "/World.json");
            World world = SDKUtil.DeserializeObject<World>(worldJson);

            UserWorld userWorld = new UserWorld();
            userWorld.cover_image = world.coverImage;
            userWorld.world_json = worldJson;
            userWorld.min_app_version = world.minAppVersion;
            userWorld.min_compat_code = world.minCompatibilityVersionCode;
            userWorld.unity_version = Application.unityVersion;
            userWorld.is_virtual = (byte) (world.isVirtual ? 1 : 0);
            userWorld.android_size = verifications[worldName].distroSizeAndroid;
            userWorld.pc_size = verifications[worldName].distroSizeStandaloneWin;
            userWorld.linux_size = verifications[worldName].distroSizeStandaloneLinux;

            // TODO: convert to SDKUtil function as well
            byte[] data = Encoding.UTF8.GetBytes(SDKUtil.SerializeObject(userWorld));
            using UnityWebRequest webRequest = UnityWebRequest.Put(uri, data);
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + GetAPIToken());
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError($"Could not update world {worldName} due to network issues: {webRequest.error}");
                uploadErrors = true;
            }
            else if (webRequest.isHttpError)
            {
                if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                {
                    SDKUtil.invalidAPIToken = true;
                    Debug.LogError("Invalid or expired API Token.");
                }
                else
                {
                    Debug.LogError($"There was an error updating world {worldName}: {webRequest.downloadHandler.text}");
                }
                uploadErrors = true;
            }
        }

        private IEnumerator FetchSpeech(string text, string filePath, Action<bool> callback)
        {
            Debug.Log("Remote (Fetch Speech)");

            string uri = SDKUtil.API_ENDPOINT + "tts/ms";

            string ssml = "<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"https://www.w3.org/2001/mstts\" " +
                          "xml:lang=\"" + TTS_LANGUAGE_CODE + "\">" +
                          "<voice name=\"" + TTS_VOICE + "\">" +
                          "<mstts:express-as type=\"" + TTS_MOOD + "\">" +
                          "<prosody pitch=\"" + TTS_PITCH + "\" " + (!string.IsNullOrEmpty(TTS_SPEED) ? "rate=\"" + TTS_SPEED + "\"" : "") + ">" +
                          SecurityElement.Escape(text) +
                          "</prosody></mstts:express-as></voice></speak>";

            byte[] data = Encoding.UTF8.GetBytes(ssml);

            using UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = SDKUtil.TIMEOUT;
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + GetAPIToken());
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError($"Could not fetch speech due to network issues: {webRequest.error}");
                speechErrors = true;
            }
            else if (webRequest.isHttpError)
            {
                if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                {
                    SDKUtil.invalidAPIToken = true;
                    Debug.LogError("Invalid or expired API Token.");
                }
                else
                {
                    Debug.LogError($"There was an error fetching speech: {webRequest.downloadHandler.text}");
                }
                speechErrors = true;
            }
            else
            {
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
            }
            callback?.Invoke(!speechErrors);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}