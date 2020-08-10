using System.IO;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class UtilityUI : BasicEditorUI
    {
        [MenuItem("traVRsal/Utilities/Convert to Piece", priority = 500)]
        public static void ConvertToPiece()
        {
            GameObject go = DoConvertToPiece();
            if (go == null) return;

            CreatePrefab(go.transform.parent.gameObject);
        }

        [MenuItem("traVRsal/Utilities/Convert to Piece - Pivot to lower corner", priority = 501)]
        public static void ConvertToPieceWithPivot()
        {
            GameObject go = DoConvertToPiece();
            if (go == null) return;

            Undo.RecordObject(go.transform, "Align pivot");
            go.transform.localPosition = new Vector3(go.transform.localScale.x / 2f, go.transform.localScale.y / 2f, go.transform.localScale.z / 2f);

            CreatePrefab(go.transform.parent.gameObject);
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
            go.transform.localPosition = Vector3.zero;

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
                CreatePrefab(go, Path.GetFileName(worlds[0]));
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