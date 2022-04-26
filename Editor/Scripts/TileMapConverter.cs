using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    public class TileMapConverter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            List<string> changedFiles = new List<string>();

            foreach (string str in importedAssets.Concat(movedAssets).Concat(movedFromAssetPaths))
            {
                if (str.EndsWith("." + TileMapUtil.MAP_EXTENSION) || str.EndsWith("." + TileMapUtil.WORLD_EXTENSION))
                {
                    changedFiles.Add(str);
                }
            }

            if (changedFiles.Count > 0)
            {
                // only run if not in play-mode as otherwise we get duplicate reloads in game
                if (!Application.isPlaying && Application.isEditor)
                {
                    ConvertTiledToJson(changedFiles);

                    // live reload in editor
                    AssetDatabase.Refresh();
                }
            }
        }

        public static void ConvertTiledToJson(List<string> changedFiles)
        {
            // TODO: use TileMapUtil
            string tiledExe = TravrsalSettingsManager.Get("tiledPath", SDKUtil.TILED_PATH_DEFAULT);
            if (string.IsNullOrEmpty(tiledExe)) return;

            foreach (string file in changedFiles)
            {
                // leave old name intact and simply add .json
                string targetName = file + ".json";

                if (file.EndsWith("." + TileMapUtil.WORLD_EXTENSION))
                {
                    FileUtil.ReplaceFile(file, targetName);
                }
                else
                {
                    Process process = new Process();
                    process.StartInfo.FileName = tiledExe;
                    // do not use --resolve-types-and-properties to have full control over declaration chain
                    process.StartInfo.Arguments = "--export-map JSON --embed-tilesets \"" + file + "\" \"" + targetName + "\"";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();
                }
            }
        }
    }
}