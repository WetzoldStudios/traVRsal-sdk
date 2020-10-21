using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace traVRsal.SDK
{
    public class ConstructorUI : BasicEditorUI
    {
        private string varName;

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

            GUILayout.BeginHorizontal();

            GUILayout.Label("New Variable:", EditorStyles.wordWrappedLabel);
            varName = GUILayout.TextField(varName);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create Variable")) CreateVariable();

            OnGUIDone();
        }


        private void CreateVariable()
        {
            if (string.IsNullOrEmpty(varName)) return;
            World world = LoadWorld();
            if (world == null) return;

            if (world.initialVariables == null) world.initialVariables = new List<Variable>();
            world.initialVariables.Add(new Variable(varName));
            varName = "";

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
                EditorUtility.DisplayDialog("Error", "There is more than one world in the project. The builder does not support multi-world projects yet.", "OK");
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