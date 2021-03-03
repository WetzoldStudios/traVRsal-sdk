using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace traVRsal.SDK
{
    public static class EDebug
    {
        public static List<Tuple<string, string>> errors = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> warnings = new List<Tuple<string, string>>();

        public static void Clear()
        {
            errors.Clear();
            warnings.Clear();
        }

        public static void LogWarning(string message, Object context = null)
        {
            warnings.Add(new Tuple<string, string>(GetDate(), message));
            Debug.LogWarning(message, context);
        }

        public static void LogError(string message, Object context = null)
        {
            errors.Add(new Tuple<string, string>(GetDate(), message));
            Debug.LogError(message, context);
        }

        public static List<string> GetData(bool returnErrors, bool returnWarnings)
        {
            List<string> result = new List<string>();

            if (returnErrors) result.AddRange(errors.Select(c => "[" + c.Item1 + "] " + c.Item2).ToList());
            if (returnWarnings) result.AddRange(warnings.Select(c => "[" + c.Item1 + "] " + c.Item2).ToList());

            result.Sort();

            return result;
        }

        private static string GetDate()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}