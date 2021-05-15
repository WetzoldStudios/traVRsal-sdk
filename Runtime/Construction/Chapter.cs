using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Chapter
    {
        public string key;
        public string name;
        public string description;
        public string cover;

        public Chapter()
        {
        }

        public override string ToString()
        {
            return $"Chapter {key}";
        }
    }
}