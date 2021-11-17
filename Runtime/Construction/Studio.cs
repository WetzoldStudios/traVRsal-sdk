using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Studio
    {
        public string projectFolder;
        public string studioPath;
        public string version;
        public string unityVersion;

        public Studio()
        {
        }

        public override string ToString()
        {
            return $"Studio Info {projectFolder}";
        }
    }
}