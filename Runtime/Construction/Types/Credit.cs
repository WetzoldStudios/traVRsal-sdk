using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class Credit
    {
        public string activity;
        public string author;
        public string originalSource;
        public string url;

        public Credit()
        {
        }

        public override string ToString()
        {
            return $"Credit {author} ({activity})";
        }
    }
}