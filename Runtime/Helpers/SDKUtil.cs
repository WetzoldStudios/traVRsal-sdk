using System;
using System.Threading;

namespace traVRsal.SDK
{
    public static class SDKUtil
    {
        public const string AUTO_GENERATED = "[AUTO]";

        public enum ColliderType
        {
            NONE, BOX, SPHERE
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