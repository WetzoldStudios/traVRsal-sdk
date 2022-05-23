using Asyncoroutine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace traVRsal.SDK
{
    public class PublishUI : BasicEditorUI
    {
        private const string PREFS_PREFIX = "[traVRsal]-[SDK]-";
        private const string TTS_LANGUAGE_CODE = "en-US";
        private const string TTS_VOICE = "en-US-GuyNeural";
        private const string TTS_MOOD = "cheerful";
        private const string TTS_PITCH = "low";
        private const string TTS_SPEED = "";
        private const string REPLICA_PREFIX = "replica-";

        private readonly string[] PACKAGE_OPTIONS = {"Everything", "Intelligent"};
        private readonly string[] RELEASE_CHANNELS = {"Production", "Beta", "Alpha"};

        private static bool _debugMode = false;

        private static bool _packagingInProgress;
        private static bool _documentationInProgress;
        private static bool _uploadErrors;
        private static bool _speechErrors;
        private static bool _uploadInProgress;
        private static bool _verifyInProgress;
        private static bool _packagingSuccessful;
        private static bool _verificationPassed;
        private static bool _uploadPossible;
        private static bool _worldListMismatch;

        private int _releaseChannel;
        private int _packageMode = 1;

        private static DateTime _uploadStartTime;
        private static float _uploadProgress = 1;
        private static int _uploadProgressId;
        private static int _uncompressedTextures;
        private static int _problematicAudio;
        private static int _storyErrorCount;
        private static DirectoryWatcher _dirWatcher;
        private static PublishUI _window;
        private static Dictionary<string, VerificationResult> _verifications = new Dictionary<string, VerificationResult>();
        private static int _preparedReleaseChannel;

        [MenuItem("traVRsal/Publisher", priority = 120)]
        public static void ShowWindow()
        {
            _window = GetWindow<PublishUI>("traVRsal Publisher");
        }

        public override void OnEnable()
        {
            base.OnEnable();
            CreateDirWatcher();

            _releaseChannel = PlayerPrefs.GetInt(PREFS_PREFIX + "Channel", _releaseChannel);
        }

        public void OnDisable()
        {
            PlayerPrefs.SetInt(PREFS_PREFIX + "Channel", _releaseChannel);
        }

        private static void CreateDirWatcher()
        {
            if (_dirWatcher == null)
            {
                _dirWatcher = new DirectoryWatcher(new FSWParams(GetWorldsRoot(false)));
                _dirWatcher.StartFSW();
            }
        }

        public override void OnGUI()
        {
            // independent of actual window
            if (_uploadInProgress)
            {
                int timeRemaining = Mathf.Max(1, Mathf.RoundToInt((DateTime.Now.Subtract(_uploadStartTime).Seconds / _uploadProgress) * (1 - _uploadProgress)));
                Progress.SetRemainingTime(_uploadProgressId, timeRemaining);
                Progress.Report(_uploadProgressId, _uploadProgress, "Uploading worlds to server...");
            }

            base.OnGUI();

            GUILayout.Label("Packaging ensures that the studio shows the most up to date version of your world.", EditorStyles.wordWrappedLabel);

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
                        _packageMode = EditorGUILayout.Popup(_packageMode, PACKAGE_OPTIONS);
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        _packageMode = 0;
                    }

                    EditorGUI.BeginDisabledGroup(_packagingInProgress || _uploadInProgress);
                    string buttonText = "Package";
                    if (_packageMode == 1)
                    {
                        string[] worldsToBuild = GetWorldsToBuild(_packageMode).Select(Path.GetFileName).ToArray();
                        buttonText += " (" + ((_dirWatcher.affectedFiles.Count > 0) ? string.Join(", ", worldsToBuild) : "everything") + ")";
                    }

                    if (GUILayout.Button(buttonText)) EditorCoroutineUtility.StartCoroutine(PackageWorlds(_packageMode, _releaseChannel, _packageMode == 2, _packageMode == 2), this);
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
                    _releaseChannel = EditorGUILayout.Popup(_releaseChannel, RELEASE_CHANNELS);

                    EditorGUI.BeginDisabledGroup(_packagingInProgress || _uploadInProgress || _verifyInProgress || _documentationInProgress);
                    if (GUILayout.Button("Prepare Upload")) EditorCoroutineUtility.StartCoroutine(PrepareUpload(), this);
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }

                CheckTokenGUI();
                if (_worldListMismatch && !SDKUtil.networkIssue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The worlds inside your Worlds folder do not match your registered worlds on www.traVRsal.com. You probably need to rename these locally to match exactly.", MessageType.Error);
                    if (GUILayout.Button("Refresh")) EditorCoroutineUtility.StartCoroutine(RefreshVerify(), this);
                }

                if (_storyErrorCount > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox($"Stories contain {_storyErrorCount} errors that need to be fixed first. See console for details.", MessageType.Error);
                    GUILayout.EndHorizontal();
                }

                if (_uncompressedTextures > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Uncompressed textures in project: " + _uncompressedTextures, MessageType.Warning);
                    if (GUILayout.Button("Fix")) EditorCoroutineUtility.StartCoroutine(CompressTextures(), this);
                    GUILayout.EndHorizontal();
                }

                if (_problematicAudio > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Non-optimal audio in project: " + _problematicAudio, MessageType.Warning);
                    if (GUILayout.Button("Fix")) EditorCoroutineUtility.StartCoroutine(OptimizeAudio(), this);
                    GUILayout.EndHorizontal();
                }

                if (_verifications.Count > 0)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Verification Results", EditorStyles.boldLabel);

                    foreach (string dir in GetWorldPaths())
                    {
                        string worldName = Path.GetFileName(dir);
                        if (!_verifications.ContainsKey(worldName)) continue;

                        VerificationResult v = _verifications[worldName];

                        v.showDetails = EditorGUILayout.Foldout(v.showDetails, worldName);

                        if (v.showDetails)
                        {
                            PrintTableRow("Size (Quest)", v.distroExistsAndroid ? SDKUtil.BytesToString(v.distroSizeAndroid) : "not packaged yet");
                            PrintTableRow("Size (Windows)", v.distroExistsStandaloneWin ? SDKUtil.BytesToString(v.distroSizeStandaloneWin) : "not packaged yet");
                            PrintTableRow("Size (Linux)", v.distroExistsStandaloneLinux ? SDKUtil.BytesToString(v.distroSizeStandaloneLinux) : "not packaged yet");
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
                            EditorGUI.BeginDisabledGroup(_packagingInProgress || _uploadInProgress || !_uploadPossible);
                            if (GUILayout.Button("Upload")) EditorCoroutineUtility.StartCoroutine(UploadWorld(worldName), this);
                            if (_debugMode && GUILayout.Button("Register")) EditorCoroutineUtility.StartCoroutine(PublishWorldUpdates(worldName), this);
                            EditorGUI.EndDisabledGroup();
                            EndPartialTableRow();
                        }
                    }
                }

                if (_debugMode)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Debug mode is enabled.", MessageType.Warning);

                    EditorGUI.BeginDisabledGroup(_packagingInProgress || _uploadInProgress || _verifyInProgress || _documentationInProgress);
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

        private static void PrintTableRow(string key, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(5));
            GUILayout.Label(key, GUILayout.Width(100));
            GUILayout.Label(value);
            GUILayout.EndHorizontal();
        }

        private static void BeginPartialTableRow(string key)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(5));
            GUILayout.Label(key, GUILayout.Width(100));
        }

        private static void EndPartialTableRow()
        {
            GUILayout.EndHorizontal();
        }

        private static void PrepareWorldFiles()
        {
            foreach (string dir in GetWorldPaths())
            {
                string worldName = Path.GetFileName(dir);
                UpdateWorldData(worldName);
            }
        }

        private static IEnumerator FetchDefaultTTS()
        {
            List<string> failedReplica = new List<string>();
            foreach (string dir in GetWorldPaths())
            {
                World world = SDKUtil.ReadJSONFileDirect<World>(dir + "/World.json");
                if (world.isVirtual) continue;

                string voicePath = dir + "/Audio/Voice/Generated/";
                Directory.CreateDirectory(voicePath);

                // generate loading texts
                string text = world.journeys?.Count > 0 ? $"Creating a random and unique play-through for {world.name}. " : $"Loading {world.name}. ";
                text += string.IsNullOrEmpty(world.longDescription) ? world.shortDescription : world.longDescription;

                string targetFile = voicePath + SDKUtil.VOICE_LOADING_WORLD;
                string targetHashFile = voicePath + SDKUtil.VOICE_LOADING_WORLD + "_" + text.GetHashString() + ".json";
                string targetHashOverrideFile = voicePath + SDKUtil.VOICE_LOADING_WORLD + ".json";
                if (!File.Exists(targetHashFile) && !File.Exists(targetHashOverrideFile))
                {
                    File.WriteAllText(targetHashFile, text.GetHashString());
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    yield return FetchDefaultSpeech(text, targetFile, result => { });
                }

                // load referenced speech fragments
                world.dependencies.referencedSpeech.Add(SDKUtil.VOICE_WORLD_LOADED);
                foreach (string speech in world.dependencies.referencedSpeech)
                {
                    targetFile = voicePath + SDKUtil.VOICE_SPEECH_PREFIX + speech.GetHashString() + ".wav";

                    // always override replica files to ensure latest is used
                    if (speech.ToLowerInvariant().StartsWith(REPLICA_PREFIX))
                    {
                        string idx = speech.Substring(REPLICA_PREFIX.Length);
                        string detailSourceFile = "/Replica/line_" + idx + ".wav";
                        string sourceFile = Application.dataPath + detailSourceFile;
                        if (File.Exists(sourceFile))
                        {
                            File.Copy(sourceFile, targetFile, true);
                        }
                        else
                        {
                            _speechErrors = true;
                            failedReplica.Add(detailSourceFile);
                        }
                    }
                    else if (!File.Exists(targetFile))
                    {
                        yield return FetchDefaultSpeech(speech, targetFile, result => { });
                    }
                }
            }

            if (failedReplica.Count > 0)
            {
                Debug.LogError("The following Replica voice-overs could not be found: " + string.Join(", ", failedReplica));
                EditorUtility.DisplayDialog("Voice-Over Errors", "The following Replica voice-overs could not be found:\n\n" +
                                                                 string.Join("\n", failedReplica), "OK");
            }
        }

        /// <summary>
        /// Download all speech, create all components and verify result at build time so that world loading is as fast as possible.
        /// </summary>
        private static IEnumerator MaterializeStories()
        {
            string transName = SDKUtil.AUTO_GENERATED + "StoryPlayer";

            _storyErrorCount = 0;
            _replicaToken = null; // force re-auth and new token each time packaging is done
            foreach (string dir in GetWorldPaths())
            {
                World world = SDKUtil.ReadJSONFileDirect<World>(dir + "/World.json");
                string worldName = Path.GetFileName(dir);
                string root = GetWorldsRoot() + "/" + worldName + "/";
                string absTargetDir = $"{dir}/Audio/Voice/Generated";
                string relTargetDir = $"Assets/Worlds/{worldName}/Audio/Voice/Generated";
                foreach (string folder in new[] {"Pieces", "Sceneries", "Logic"})
                {
                    string[] assets = AssetDatabase.FindAssets("*", new[] {root + folder});

                    int progressId = Progress.Start("Scanning prefabs for stories");
                    int current = 0;
                    int total = assets.Length;

                    foreach (string asset in assets)
                    {
                        current++;
                        string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                        if (!assetPath.ToLower().EndsWith(".prefab")) continue;
                        Progress.Report(progressId, (float) current / total, assetPath);

                        GameObject go = PrefabUtility.LoadPrefabContents(assetPath);
                        bool changed = false;
                        int varChannel = 0;
                        StoryPlayer[] players = go.GetComponentsInChildren<StoryPlayer>(true);
                        foreach (StoryPlayer story in players)
                        {
                            // recreate logic root
                            Transform autoRoot = story.transform.Find(transName);
                            if (autoRoot != null) DestroyImmediate(autoRoot.gameObject);
                            if (story.file == null) continue;

                            // parse story script
                            StoryScript script = new StoryScript(story.file.text);
                            if (script.actions.Count == 0) continue;

                            // establish default components
                            autoRoot = new GameObject(transName).transform;
                            autoRoot.parent = story.transform;
                            GameObject autoRootGo = autoRoot.gameObject;
                            AudioSource audioSource = autoRootGo.AddComponent<AudioSource>();
                            audioSource.playOnAwake = false;

                            int subProgressId = Progress.Start("Materializing story", null, Progress.Options.None, progressId);
                            int subCurrent = 0;
                            int subTotal = script.GetActionCount();

                            UnityEvent lastChain = null;
                            foreach (StoryAction action in script.actions)
                            {
                                subCurrent++;

                                // one GO per action
                                GameObject actionGo = new GameObject(action.ToString());
                                actionGo.transform.parent = autoRoot;

                                UnityEvent nextChain = null;
                                object obj = null;
                                object param = null;
                                MethodInfo targetInfo = null;
                                bool keepChain = false; // true in case component is an action only and next one should follow immediately
                                switch (action.type)
                                {
                                    case StoryAction.LineType.Pause:
                                        Delay delay = actionGo.AddComponent<Delay>();
                                        delay.mode = Delay.Mode.Manual;
                                        delay.duration = action.duration;

                                        delay.onCompletion = new UnityEvent(); // otherwise not initialized yet
                                        nextChain = delay.onCompletion;
                                        obj = delay;
                                        break;

                                    case StoryAction.LineType.WaitForZone:
                                        ZoneRestriction zr = actionGo.AddComponent<ZoneRestriction>();
                                        zr.filter = action.param;
                                        zr.onBecomeActive = new UnityEvent(); // otherwise not initialized yet
                                        nextChain = zr.onBecomeActive;
                                        obj = zr;
                                        break;

                                    case StoryAction.LineType.WaitForVariable:
                                        VariableListener vl = actionGo.AddComponent<VariableListener>();
                                        vl.variable = action.param;
                                        vl.variableChannel = varChannel;
                                        vl.onTrue = new UnityEvent(); // otherwise not initialized yet
                                        nextChain = vl.onTrue;
                                        obj = vl;
                                        varChannel++;
                                        break;

                                    case StoryAction.LineType.SetVariable:
                                        SetVariable sv = actionGo.AddComponent<SetVariable>();
                                        if (bool.TryParse(action.value, out bool boolVal))
                                        {
                                            string method = boolVal ? "ReachActionMax" : "ReachActionMin";
                                            targetInfo = UnityEventBase.GetValidMethodInfo(sv, method, new[] {typeof(string)});
                                            param = action.param;
                                        }
                                        else
                                        {
                                            _storyErrorCount++;
                                            EDebug.LogError($"Could not parse boolean value (true/false) in {story} on line {action.lineNr}: {action.value}");
                                        }
                                        obj = sv;
                                        keepChain = true;
                                        break;

                                    case StoryAction.LineType.IncVariable:
                                    case StoryAction.LineType.DecVariable:
                                        SetVariable sv2 = actionGo.AddComponent<SetVariable>();
                                        string method2 = action.type == StoryAction.LineType.IncVariable ? "Increase" : "Decrease";
                                        targetInfo = UnityEventBase.GetValidMethodInfo(sv2, method2, new[] {typeof(string)});
                                        param = action.param;
                                        obj = sv2;
                                        keepChain = true;
                                        break;

                                    case StoryAction.LineType.Speech:
                                        VoiceSpec vs = world.voices?.Where(v => v.key == action.speaker).FirstOrDefault();
                                        if (vs == null)
                                        {
                                            _storyErrorCount++;
                                            EDebug.LogError($"{story} contains undefined voice reference on line {action.lineNr}: {action.raw}");
                                            continue;
                                        }

                                        if (!Directory.Exists(absTargetDir)) Directory.CreateDirectory(absTargetDir);
                                        string ssml = vs.GetSSML(action.param);
                                        string hash = vs.GetHashedFileName(ssml);
                                        string hashFile = $"{hash}.wav";
                                        action.filePath = $"{absTargetDir}/{hashFile}";
                                        if (!File.Exists(action.filePath))
                                        {
                                            switch (vs.backend)
                                            {
                                                case VoiceSpec.TTSBackend.Replica:
                                                    yield return FetchReplicaSpeech(ssml, vs.voice, action.filePath, success =>
                                                    {
                                                        if (!success)
                                                        {
                                                            _storyErrorCount++;
                                                            Debug.LogError($"Could not successfully generate voice file for Replica line: {action}");
                                                        }
                                                    });
                                                    break;

                                                default:
                                                    _storyErrorCount++;
                                                    Debug.LogError($"{vs} references TTS backend '{vs.backend}' which is not yet supported in stories");
                                                    break;
                                            }
                                            AssetDatabase.Refresh();
                                        }
                                        SpeechPlayer player = actionGo.AddComponent<SpeechPlayer>();
                                        player.subtitle = vs.GetRawText(action.param);
                                        player.speaker = action.speaker;
                                        player.clip = AssetDatabase.LoadAssetAtPath($"{relTargetDir}/{hashFile}", typeof(AudioClip)) as AudioClip;

                                        player.onDone = new UnityEvent(); // otherwise not initialized yet
                                        nextChain = player.onDone;
                                        obj = player;

                                        break;
                                }
                                if (obj != null)
                                {
                                    if (targetInfo == null) targetInfo = UnityEventBase.GetValidMethodInfo(obj, "Trigger", Type.EmptyTypes);
                                    if (targetInfo != null) // not all logic components are directly triggered, e.g. zone restrictions, variable listeners...
                                    {
                                        UnityAction ua = Delegate.CreateDelegate(typeof(UnityAction), obj, targetInfo, false) as UnityAction;

                                        if (lastChain != null)
                                        {
                                            if (param is string stringParam)
                                            {
                                                UnityAction<string> ua2 = Delegate.CreateDelegate(typeof(UnityAction<string>), obj, targetInfo, false) as UnityAction<string>;
                                                UnityEventTools.AddStringPersistentListener(lastChain, ua2, stringParam);
                                            }
                                            else
                                            {
                                                UnityEventTools.AddPersistentListener(lastChain, ua);
                                            }
                                        }
                                        else if (subCurrent == 1)
                                        {
                                            // if first item, remove all old trigger listeners then link from story
                                            for (int i = story.onTrigger.GetPersistentEventCount() - 1; i >= 0; i--)
                                            {
                                                UnityEventTools.RemovePersistentListener(story.onTrigger, i);
                                            }
                                            UnityEventTools.AddPersistentListener(story.onTrigger, ua);
                                        }
                                    }
                                }
                                if (!keepChain) lastChain = nextChain;
                                Progress.Report(subProgressId, (float) subCurrent / subTotal, action.raw);
                            }

                            Progress.Remove(subProgressId);
                            changed = true;
                        }

                        if (changed) PrefabUtility.SaveAsPrefabAsset(go, assetPath);
                        PrefabUtility.UnloadPrefabContents(go);

                        yield return null;
                    }
                    Progress.Remove(progressId);
                }
            }
        }

        private IEnumerator PrepareUpload(bool force = false, bool linuxOnly = false)
        {
            _preparedReleaseChannel = _releaseChannel;

            yield return FetchUserWorlds();
            PrepareWorldFiles();

            yield return PackageWorlds(_packageMode, _releaseChannel, true, true, force, linuxOnly);
            yield return CreateDocumentation();
            PrepareCommonFiles();
            Verify();

            _uploadPossible = _packagingSuccessful && _verificationPassed;
        }

        private static void PrepareCommonFiles()
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

        private static IEnumerable<AudioImporter> GetProblematicAudio()
        {
            return AssetDatabase.FindAssets("t:audioclip", null)
                .Select(guid => AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as AudioImporter)
                .Where(ai => ai != null)
                .Where(ai => !ai.assetPath.StartsWith("Packages/"))
                .Where(ai => !ai.assetPath.Contains("/Editor/"))
                .Where(ai => ai.ContainsSampleSettingsOverride("Android")
                             || ai.defaultSampleSettings.sampleRateSetting == AudioSampleRateSetting.PreserveSampleRate
                             || ai.defaultSampleSettings.compressionFormat != AudioCompressionFormat.Vorbis
                             || ai.defaultSampleSettings.quality > 0.5f
                             || (ai.assetPath.Contains("/Music/") && !ai.loadInBackground));
        }

        private static IEnumerable<TextureImporter> GetUncompressedTextures()
        {
            return AssetDatabase.FindAssets("t:texture", null)
                .Select(guid => AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as TextureImporter)
                .Where(ti => ti != null)
                .Where(ti => !ti.assetPath.StartsWith("Packages/"))
                .Where(ti => !ti.assetPath.Contains("/Editor/"))
                .Where(ti => ti.textureCompression == TextureImporterCompression.Uncompressed || !ti.crunchedCompression);
        }

        private static IEnumerator CompressTextures()
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

            _uncompressedTextures = 0;

            Progress.Remove(progressId);
            EditorUtility.DisplayDialog("Done", "Texture compression completed. Prepare the upload again.", "OK");
        }

        private static IEnumerator OptimizeAudio()
        {
            IEnumerable<AudioImporter> uncompressed = GetProblematicAudio();

            int progressId = Progress.Start("Compressing project audio");
            int current = 0;
            int total = uncompressed.Count();
            foreach (AudioImporter ai in uncompressed)
            {
                current++;
                Progress.Report(progressId, (float) current / total, ai.assetPath);

                if (ai.assetPath.Contains("/Music")) ai.loadInBackground = true;
                ai.ClearSampleSettingOverride("Android");

                AudioImporterSampleSettings settings = ai.defaultSampleSettings;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                if (settings.quality > 0.5f) settings.quality = 0.5f;
                if (settings.sampleRateSetting == AudioSampleRateSetting.PreserveSampleRate) settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
                ai.defaultSampleSettings = settings;

                AssetDatabase.ImportAsset(ai.assetPath);

                yield return null;
            }

            _problematicAudio = 0;

            Progress.Remove(progressId);
            EditorUtility.DisplayDialog("Done", "Audio optimization completed. Prepare the upload again.", "OK");
        }

        private void Verify()
        {
            _verifyInProgress = true;
            _verificationPassed = false;
            _worldListMismatch = false;
            _verifications.Clear();

            IEnumerable<TextureImporter> uncompressed = GetUncompressedTextures();
            _uncompressedTextures = uncompressed.Count();

            IEnumerable<AudioImporter> audioProblems = GetProblematicAudio();
            _problematicAudio = audioProblems.Count();

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

                _verifications.Add(worldName, result);

                if (userWorlds != null && userWorlds.Count(w => w.key == worldName) == 0)
                {
                    Debug.LogError($"Found unregistered world: {worldName}");
                    _worldListMismatch = true;
                }
            }

            _verifyInProgress = false;
            _verificationPassed = !_worldListMismatch && !SDKUtil.invalidAPIToken && !SDKUtil.networkIssue && _storyErrorCount == 0;
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
                        worldName => _dirWatcher.affectedFiles.Any(af => af.Contains("/" + Path.GetFileName(worldName) + "/") || af.Contains("\\" + Path.GetFileName(worldName) + "\\"))
                    ).ToArray();
                    if (filteredWorlds.Length > 0) worldsToBuild = filteredWorlds;
                }
                catch
                {
                    // ignored
                }
            }

            return worldsToBuild;
        }

        public static IEnumerator PackageWorlds(int packageMode, int releaseChannel, bool allWorlds, bool allTargets, bool force = false, bool linuxOnly = false)
        {
            _uploadPossible = false;
            _packagingInProgress = true;
            _packagingSuccessful = false;

            PrepareWorldFiles(); // prepare again as in normal packaging world analysis and object keys would otherwise not be available instantly
            yield return FetchDefaultTTS();
            yield return MaterializeStories();
            yield return IndexFolders();

            if (_storyErrorCount == 0)
            {
                try
                {
                    string[] worldsToBuild = allWorlds ? GetWorldPaths() : GetWorldsToBuild(packageMode);
                    if (worldsToBuild.Length == 0) yield break;
                    string resultFolder = Application.dataPath + "/../traVRsal/";
                    BuildTarget mainTarget = linuxOnly || Application.platform == RuntimePlatform.LinuxEditor ? BuildTarget.StandaloneLinux64 : BuildTarget.StandaloneWindows64;

                    CreateLockFile();
                    ConvertTileMaps();
                    CreateAddressableSettings(!allTargets, releaseChannel);
                    PlayerSettings.colorSpace = ColorSpace.Linear;
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
                            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64)) targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
                            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64)) targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64));
                        }
                        else
                        {
                            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64)) targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64));
                            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64)) targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
                        }
                    }
                    else
                    {
                        targets.Add(new Tuple<BuildTargetGroup, BuildTarget>(BuildTargetGroup.Standalone, mainTarget));
                    }

                    // iterate over all supported platforms
                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                    foreach (Tuple<BuildTargetGroup, BuildTarget> target in targets)
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTarget(target.Item1, target.Item2);

                        // build each world individually
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
                    _packagingSuccessful = true;
                }
                catch (Exception e)
                {
                    _packagingInProgress = false;
                    EditorUtility.DisplayDialog("Error", $"Packaging could not be completed. Error: {e.Message}", "Close");
                    yield break;
                }
            }

            if (_dirWatcher != null)
            {
                _dirWatcher.ClearAffected(); // only do at end, since during build might cause false positives
            }
            else
            {
                CreateDirWatcher(); // can happen after initial project creation
            }

            RemoveLockFile();
            _packagingInProgress = false;

            Debug.Log("Packaging completed successfully.");
        }

        private static string GetDocuPath(string worldName)
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

            DiscoverObjects(world, root);
            AddRuntimeAnalytics(world, root);
            IncreaseVersion(world);

            // write back
            world.NullifyEmpties();
            File.WriteAllText(root + "World.json", SDKUtil.SerializeObject(world, DefaultValueHandling.Ignore));

            return true;
        }

        private static void DiscoverObjects(World world, string root)
        {
            string worldRootPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(AssetDatabase.AssetPathToGUID(root + "World.json")));
            world.logic = new List<string>();
            world.objectSpecs = new List<ObjectSpec>();
            world.usedTags = new HashSet<string>();
            foreach (string folder in new[] {"Pieces", "Sceneries", "Logic"})
            {
                string[] assets = AssetDatabase.FindAssets("*", new[] {root + folder});
                foreach (string asset in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                    if (!assetPath.ToLower().EndsWith(".prefab")) continue;

                    // construct key
                    string worldBasePath = worldRootPath + "/" + folder;
                    string prefix = (folder == "Sceneries" ? folder + "/" : "") + Path.GetDirectoryName(assetPath);
                    if (!string.IsNullOrEmpty(prefix) && prefix.Length > worldBasePath.Length)
                    {
                        prefix = prefix.Substring(worldBasePath.Length + 1) + "/";
                        prefix = prefix.Replace('\\', '/');
                    }
                    else
                    {
                        prefix = "";
                    }
                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    string objectKey = prefix + fileName;

                    if (folder == "Logic")
                    {
                        world.logic.Add(objectKey);
                        continue;
                    }

                    GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
                    if (!string.IsNullOrWhiteSpace(prefab.tag) && !prefab.CompareTag("Untagged")) world.usedTags.Add(prefab.tag);

                    if (prefab.TryGetComponent(out ExtendedAttributes ea))
                    {
                        if (!ea.spec.IsDefault())
                        {
                            ea.spec.objectKey = objectKey;
                            world.objectSpecs.Add(ea.spec);
                        }
                    }

                    if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        private static void AddRuntimeAnalytics(World world, string root)
        {
            WorldAnalysis analysis = SDKUtil.ReadJSONFileDirect<WorldAnalysis>(root + "Data/WorldAnalysis.json");
            world.dependencies = analysis;
        }

        private static void IncreaseVersion(World world)
        {
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
                    Debug.LogError($"World.json of {world.key} contained an unreadable version. Resetting to {world.version}.");
                }
            }

            world.versionCode++;
        }

        private static string GetDocuArchivePath(string worldName)
        {
            return GetDocuPath(worldName) + "../" + worldName + "-docs.zip";
        }

        private static IEnumerator IndexFolders()
        {
            foreach (string dir in GetWorldPaths())
            {
                string[] folders = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
                foreach (string folder in folders)
                {
                    string catalogFile = folder + "/" + SDKUtil.FILE_LISTING;
                    FileUtil.DeleteFileOrDirectory(catalogFile);

                    IEnumerable<string> files = DirectoryUtil.GetFiles(folder, new[] {"*.*"});
                    List<FileDetails> result = new List<FileDetails>();
                    foreach (string file in files)
                    {
                        if (Directory.Exists(file)) continue; // skip directories
                        FileInfo fi = new FileInfo(file);
                        if (fi.Extension == ".meta") continue;

                        result.Add(new FileDetails(fi.Name, fi.Length, fi.CreationTimeUtc, fi.LastWriteTimeUtc));
                    }
                    if (result.Count > 0) File.WriteAllText(catalogFile, SDKUtil.SerializeObject(result, DefaultValueHandling.Ignore));
                }
                yield return null;
            }
        }

        private IEnumerator CreateDocumentation()
        {
            _documentationInProgress = true;

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
                    string[] assets = Array.Empty<string>();
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

            _documentationInProgress = false;
        }

        private static string GetLockFileLocation()
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
            settings.DisableCatalogUpdateOnStartup = false;
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

                SetProfileValue(profile, profileId, AddressableAssetSettings.kLocalBuildPath, localRoot);
                SetProfileValue(profile, profileId, AddressableAssetSettings.kLocalLoadPath, localRoot);
                SetProfileValue(profile, profileId, AddressableAssetSettings.kRemoteBuildPath, $"ServerData/Worlds/{worldName}/[BuildTarget]");
                SetProfileValue(profile, profileId, AddressableAssetSettings.kRemoteLoadPath, $"{remoteTarget}Worlds/{worldName}/[BuildTarget]");

                // ensure correct group settings
                BundledAssetGroupSchema groupSchema = group.GetSchema<BundledAssetGroupSchema>();
                groupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenWhenNewVersionLoaded;
                groupSchema.ForceUniqueProvider = true;
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

        private static void SetProfileValue(AddressableAssetProfileSettings profile, string profileId, string variableName, string value)
        {
            List<string> variables = profile.GetVariableNames();
            if (!variables.Contains(variableName)) profile.CreateValue(variableName, value);
            profile.SetValue(profileId, variableName, value);
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

            _uploadErrors = false;
            _uploadInProgress = true;
            _uploadProgress = 0;
            _uploadStartTime = DateTime.Now;
            _uploadProgressId = Progress.Start("Uploading world");

            AWSUtil aws = new AWSUtil();
            yield return aws.UploadDirectory(GetServerDataPath(), progress => _uploadProgress = progress, "Worlds/" + worldName + "/*").AsCoroutine();
            yield return PublishWorldUpdates(worldName);

            Progress.Remove(_uploadProgressId);
            _uploadInProgress = false;

            if (_uploadErrors)
            {
                EditorUtility.DisplayDialog("Error", "Upload could not be completed due to errors. Check the console for details.", "OK");
            }
            else
            {
                string channelName = null;
                switch (_preparedReleaseChannel)
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
            switch (_preparedReleaseChannel)
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
            userWorld.android_size = _verifications[worldName].distroSizeAndroid;
            userWorld.pc_size = _verifications[worldName].distroSizeStandaloneWin;
            userWorld.linux_size = _verifications[worldName].distroSizeStandaloneLinux;

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
                _uploadErrors = true;
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

                _uploadErrors = true;
            }
        }

        private static IEnumerator FetchDefaultSpeech(string text, string filePath, Action<bool> callback)
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
                _speechErrors = true;
            }
            else if (webRequest.isHttpError)
            {
                if (webRequest.responseCode == (int) HttpStatusCode.Unauthorized)
                {
                    SDKUtil.invalidAPIToken = true;
                    Debug.LogError("Invalid or expired API Token");
                }
                else
                {
                    Debug.LogError($"There was an error fetching speech: {webRequest.downloadHandler.text}");
                }

                _speechErrors = true;
            }
            else
            {
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
            }

            callback?.Invoke(!_speechErrors);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}