using System;
using System.Threading;

namespace traVRsal.SDK
{
    public static class SDKUtil
    {
        public const string PACKAGE_NAME = "com.wetzold.travrsal.sdk";
        public const string AUTO_GENERATED = "[AUTO]";
        public const string TILED_PATH_DEFAULT = "C:\\Program Files\\Tiled\\tiled.exe";

        // Tags
        public const string INTERACTABLE_TAG = "Interactable";
        public const string COLLECTIBLE_TAG = "Collectible";
        public const string PLAYER_TAG = "Player";
        public const string PLAYER_HEAD_TAG = "Player Head";
        public const string PLAYER_HELPER_TAG = "Player Helper";
        public const string ENEMY_TAG = "Enemy";

        public enum ColliderType
        {
            NONE, BOX, SPHERE
        }

        // inspired from https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        public static string BytesToString(long byteCount)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0) return "0" + suffix[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString() + suffix[place];
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}