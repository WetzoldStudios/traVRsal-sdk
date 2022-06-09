// adapted from https://stackoverflow.com/questions/3754118/how-to-filter-directory-enumeratefiles-with-multiple-criteria

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace traVRsal.SDK
{
    public static class IOUtils
    {
        // Regex version
        public static IEnumerable<string> GetFiles(string path, string searchPatternExpression = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                .Where(file =>
                    reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel()
                .SelectMany(searchPattern =>
                    Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public static void Copy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName)) Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    Copy(subDir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static long GetSize(string path)
        {
            if (!Directory.Exists(path)) return 0;

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            long size = 0;

            foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }

            return size;
        }

        public static void DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting file '{path}': {e.Message}");
            }
        }

        public static void WriteFile(string fileName, string text)
        {
            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllText(fileName, text);
        }

        public static void WriteAllBytes(string fileName, byte[] bytes)
        {
            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllBytes(fileName, bytes);
        }

        public static async void WriteFileAsync(string fileName, string contents)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(fileName);
                await writer.WriteAsync(contents);
            }
            catch (Exception e)
            {
                Debug.Log($"Error writing to {fileName}: " + e.Message);
            }
        }

        public static async void WriteFileAsync(string fileName, byte[] contents)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(fileName);
                await writer.BaseStream.WriteAsync(contents, 0, contents.Length);
            }
            catch (Exception e)
            {
                Debug.Log($"Error writing to {fileName}: " + e.Message);
            }
        }

        private static async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
        {
            try
            {
                using Stream source = File.OpenRead(sourcePath);
                using Stream destination = File.Create(destinationPath);
                await source.CopyToAsync(destination);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"Error writing to {destinationPath}: " + e.Message);
                return false;
            }
        }

        public static async Task<string> ReadFileAsync(string fileName)
        {
            try
            {
                using StreamReader reader = new StreamReader(fileName);
                return await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to read from {fileName}: " + e.Message);
                return null;
            }
        }
    }
}