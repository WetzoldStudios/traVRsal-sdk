using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace traVRsal.SDK
{
    public static class StringExt
    {
        public static readonly char[] DefaultSeparators = {',', ';'};

        public static string ReplaceIgnoreCase(this string input, string oldValue, string newValue)
        {
            return Regex.Replace(input, oldValue, newValue, RegexOptions.IgnoreCase);
        }

        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }

        public static string AsString(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static string TimeAgo(this DateTime dateTime)
        {
            string result;

            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);
            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = $"{timeSpan.Seconds} seconds ago";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ? $"about {timeSpan.Minutes} minutes ago" : "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ? $"about {timeSpan.Hours} hours ago" : "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ? $"about {timeSpan.Days} days ago" : "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ? $"about {timeSpan.Days / 30} months ago" : "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ? $"about {timeSpan.Days / 365} years ago" : "about a year ago";
            }

            return result;
        }

        // based on https://stackoverflow.com/questions/16606552/compress-and-decompress-string-in-c-sharp
        public static async Task<byte[]> Compress(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            using MemoryStream mso = new MemoryStream();
            using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
            {
                await gs.WriteAsync(bytes, 0, bytes.Length);
            }
            return mso.ToArray();
        }

        public static async Task<string> Decompress(byte[] data)
        {
            // Read the last 4 bytes to get the length
            byte[] lengthBuffer = new byte[4];
            Array.Copy(data, data.Length - 4, lengthBuffer, 0, 4);
            int uncompressedSize = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] buffer = new byte[uncompressedSize];
            using MemoryStream ms = new MemoryStream(data);
            using GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
            await gzip.ReadAsync(buffer, 0, uncompressedSize);
            return Encoding.Unicode.GetString(buffer);
        }

        public static string[] SplitLines(this string text)
        {
            return text.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            );
        }

        public static string RandomString(int length)
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = CHARS[ThreadSafeRandom.ThisThreadsRandom.Next(CHARS.Length)];
            }

            return new string(stringChars);
        }

        public static string[] CleanSplit(string input, char separator = ',')
        {
            return input?.Split(separator).Select(str => str.Trim()).ToArray();
        }

        public static string[] CleanSplit(string input, char[] separators)
        {
            return input?.Split(separators).Select(str => str.Trim()).ToArray();
        }

        public static string[] ParseContentAssignment(string contents, int slotCount)
        {
            if (string.IsNullOrEmpty(contents)) return null;

            // only one * is supported
            if (contents.IndexOf('*') != contents.LastIndexOf('*')) return null;

            List<string> result = new List<string>();

            string[] arr = contents.Split(DefaultSeparators);
            for (int i = 0; i < arr.Length; i++)
            {
                string content = arr[i].Trim();
                if (content.StartsWith("*"))
                {
                    // * cannot be first element
                    if (i == 0) return null;

                    // repeat element or fill with optional specified content
                    string element = content.Length == 1 ? result[result.Count - 1] : content.Substring(1);
                    for (int i2 = 0; i2 < slotCount - (arr.Length - i) - i + 1; i2++)
                    {
                        result.Add(element);
                    }
                }
                else
                {
                    result.Add(content);
                }
            }

            // fill up remainder with last element
            for (int i = result.Count; i < slotCount; i++)
            {
                result.Add(result[i - 1]);
            }

            return result.ToArray();
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static Vector2 Str2Vector2(string value, Dictionary<string, float> units = null)
        {
            string[] arr = CleanSplit(value, DefaultSeparators);
            if (arr == null || arr.Length < 2) return Vector2.zero;

            float x = Str2Float(arr[0], units);
            float y = Str2Float(arr[1], units);
            return new Vector2(x, y);
        }

        public static Vector3 Str2Vector3(string value, Dictionary<string, float> units = null)
        {
            string[] arr = CleanSplit(value, DefaultSeparators);
            if (arr == null || arr.Length < 3) return Vector3.zero;

            float x = Str2Float(arr[0], units);
            float y = Str2Float(arr[1], units);
            float z = Str2Float(arr[2], units);
            return new Vector3(x, y, z);
        }

        public static float Str2Float(string value, Dictionary<string, float> units = null)
        {
            // process units
            float multiplier = 1f;
            KeyValuePair<string, float>? unit = units?.FirstOrDefault(v => value.EndsWith(v.Key));
            if (unit != null && !string.IsNullOrWhiteSpace(unit.Value.Key))
            {
                multiplier = unit.Value.Value;
                value = value.Substring(0, value.Length - unit.Value.Key.Length);
            }

            float.TryParse(value, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float result);
            return result * multiplier;
        }

        public static Vector2Int Str2Vector2Int(string value, char separator = ';')
        {
            string[] arr = CleanSplit(value, separator);
            if (arr == null || arr.Length < 2) return Vector2Int.zero;

            int.TryParse(arr[0], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int x);
            int.TryParse(arr[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int y);
            return new Vector2Int(x, y);
        }
    }
}