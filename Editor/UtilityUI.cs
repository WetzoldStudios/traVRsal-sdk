﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class UtilityUI : BasicEditorUI
    {
        [MenuItem("traVRsal/Utilities/Convert Selected to Piece", false, 1000)]
        public static void ConvertToPiece()
        {
            GameObject go = DoConvertToPiece();
            if (go == null) return;

            CreatePrefab(go.transform.parent.gameObject);
        }

        [MenuItem("traVRsal/Utilities/Replace Project Shaders with traVRsal Shaders", false, 1100)]
        public static void ReplaceProjectShaders()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(DoReplaceProjectShaders());
        }

        private static IEnumerator DoReplaceProjectShaders()
        {
            IEnumerable<string> paths = AssetDatabase.FindAssets("t:Material")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrEmpty(path))
                .Where(path => path.ToLowerInvariant().EndsWith(".mat"))
                .Where(path => !path.StartsWith("Packages/"))
                .Where(path => !path.Contains("/Editor/"));

            Shader litShader = Shader.Find("MazeVR/StencilObject");
            Shader simpleLitShader = Shader.Find("MazeVR/StencilObjectSimple");
            Shader unlitShader = Shader.Find("MazeVR/StencilObjectUnlit");

            if (litShader == null || simpleLitShader == null || unlitShader == null)
            {
                Debug.LogError("Required shaders not found. Check SDK integrity.");
                yield break;
            }

            int progressId = Progress.Start("Swapping shaders");
            int current = 0;
            int lit = 0;
            int doneLit = 0;
            int simpleLit = 0;
            int doneSimpleLit = 0;
            int unlit = 0;
            int doneUnlit = 0;
            int transparent = 0;
            int total = paths.Count();

            foreach (string path in paths)
            {
                current++;

                Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
                Progress.Report(progressId, (float) current / total, m.name);
                if (IsTransparent(m))
                {
                    transparent++;
                    continue;
                }

                bool adjustMaterial = false;
                switch (m.shader.name)
                {
                    case "MazeVR/StencilObject":
                        doneLit++;
                        break;

                    case "MazeVR/StencilObjectSimple":
                        doneSimpleLit++;
                        break;

                    case "MazeVR/StencilObjectUnlit":
                        doneUnlit++;
                        break;

                    case "Universal Render Pipeline/Lit":
                        m.shader = litShader;
                        adjustMaterial = true;
                        lit++;
                        break;

                    case "Universal Render Pipeline/Simple Lit":
                        m.shader = simpleLitShader;
                        adjustMaterial = true;
                        simpleLit++;
                        break;

                    case "Universal Render Pipeline/Unlit":
                        m.shader = unlitShader;
                        adjustMaterial = true;
                        unlit++;
                        break;
                }
                if (adjustMaterial)
                {
                    // m.renderQueue = 1500; // not needed anymore if stencil disabled per default
                    m.SetInt("_StencilComparison", 0); // disabled
                    m.SetInt("_StencilReference", 0); // always visible initially
                }

                yield return null;
            }
            int alreadyDone = doneLit + doneSimpleLit + doneUnlit;
            int remainder = total - lit - simpleLit - unlit - alreadyDone;

            AssetDatabase.Refresh();
            Progress.Remove(progressId);
            EditorUtility.DisplayDialog("Done", $"Materials converted: {lit} lit, {simpleLit} simple lit, {unlit} unlit.\n\n{remainder} unsupported materials, with {transparent} being transparent. {alreadyDone} were already converted.", "OK");
        }

        private static bool IsTransparent(Material m)
        {
            return m.HasProperty("_Surface") && m.GetFloat("_Surface") == 1f;
        }

        [MenuItem("traVRsal/Utilities/Create or Update Folder Image List...", false, 1101)]
        public static void CreateImageList()
        {
            string path = EditorUtility.OpenFolderPanel("Select Image Folder", "", "");
            if (string.IsNullOrEmpty(path)) return;

            // read existing mod file if existent
            ModdingData md = new ModdingData();
            string modFile = path + "/" + SDKUtil.MODFILE_NAME;
            if (File.Exists(modFile)) md = SDKUtil.ReadJSONFileDirect<ModdingData>(modFile);
            if (md.imageData == null) md.imageData = new List<ImageData>();

            // gather images
            IEnumerable<string> files = DirectoryUtil.GetFiles(path, new[] {"*.png", "*.jpg"});
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (md.imageData.Any(id => id.imageLink == fileName)) continue;

                string name = Path.GetFileNameWithoutExtension(fileName);
                if (DateTime.TryParseExact(SDKUtil.MaxLength(name, 19), "yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    name = dateTime.ToLongDateString() + " " + dateTime.ToShortTimeString();
                }
                md.imageData.Add(new ImageData(fileName, name));
            }

            File.WriteAllText(modFile, SDKUtil.SerializeObject(md));

            EditorUtility.DisplayDialog("Done", $"Found {md.imageData.Count} images. Check modding.json in target folder.", "OK");
        }

        private static GameObject DoConvertToPiece()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("Error", "Select a game object first to convert.", "OK");
                return null;
            }

            GameObject newGo = new GameObject(go.name);
            Undo.RegisterCreatedObjectUndo(newGo, "Created new Piece parent object");

            if (go.transform.parent != null) newGo.transform.parent = go.transform.parent;

            Undo.SetTransformParent(go.transform, newGo.transform, "Set new parent");
            go.transform.parent = newGo.transform;

            Undo.RecordObject(go.transform, "Adjust position of original object");
            Vector3 originalPosition = go.transform.localPosition;
            go.transform.localPosition = Vector3.zero;
            newGo.transform.localPosition = originalPosition;

            return go;
        }

        private static void CreatePrefab(GameObject go)
        {
            string[] worlds = GetWorldPaths();
            if (worlds.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "There are no worlds yet to save this piece to.", "OK");
            }
            else if (worlds.Length > 1)
            {
                EditorUtility.DisplayDialog("Error", "You manage more than one world so automatic saving is not possible. Drag the new game object manually into the correct Pieces folder to create a prefab out of it.", "OK");
            }
            else
            {
                Vector3 oldPos = go.transform.position;
                go.transform.position = Vector3.zero;

                CreatePrefab(go, Path.GetFileName(worlds[0]));

                go.transform.position = oldPos;
            }
        }

        private static void CreatePrefab(GameObject go, string worldName)
        {
            string localPath = GetWorldsRoot() + $"/{worldName}/Pieces/{go.name}.prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(go, localPath, InteractionMode.UserAction);
        }
    }
}