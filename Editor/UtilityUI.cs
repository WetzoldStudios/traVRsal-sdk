using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class UtilityUI : BasicEditorUI
    {
        [MenuItem("traVRsal/Utilities/Convert Selected to Piece")]
        public static void ConvertToPiece()
        {
            GameObject go = DoConvertToPiece();
            if (go == null) return;

            CreatePrefab(go.transform.parent.gameObject);
        }

        [MenuItem("traVRsal/Utilities/Create or Update Folder Image List...")]
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