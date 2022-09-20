using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    public static class TileMapUtil
    {
        public const string WorldExtension = "world";
        public const string MapExtension = "tmx";
        public const string TileSetExtension = "tsx";
        public const string TemplateExtension = "tx";

        public static void ConvertTileMaps(List<string> files, string tiledExecutable)
        {
            if (!File.Exists(tiledExecutable))
            {
                Debug.LogError($"Tiled.exe could not be found under the path provided: {tiledExecutable}");
                return;
            }

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                string name = file.ToLower();

                // leave old name intact and simply add .json
                string targetName = file + ".json";

                if (name.EndsWith(WorldExtension))
                {
                    File.Copy(file, targetName, true);
                }
                else if (name.EndsWith(MapExtension))
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = tiledExecutable;
                    // do not use --resolve-types-and-properties to have full control over declaration chain
                    process.StartInfo.Arguments = "--export-map JSON --embed-tilesets \"" + file + "\" \"" + targetName + "\"";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();
                }
            }
        }

        public static void TileMapToImage(string fileName, string targetName, string converterExecutable)
        {
            if (!File.Exists(converterExecutable))
            {
                Debug.LogError($"tmxrasterizer.exe could not be found under the path provided: {converterExecutable}");
                return;
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = converterExecutable;
            process.StartInfo.Arguments = "-t 200 \"" + fileName + "\" \"" + targetName + "\"";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }
}
