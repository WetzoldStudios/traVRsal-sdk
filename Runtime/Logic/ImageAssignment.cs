using UnityEngine;

namespace traVRsal.SDK
{
    public class ImageAssignment : ExecutorConfig, IDataSource
    {
        public enum ImageSource
        {
            Name = 0,
            URL = 3,
            ImageProvider = 2,
            Variable = 1
        }

        public int materialIndex;
        public bool random;
        public int order;
        public ImageSource source = ImageSource.Name;
        public string key;
        public bool adjustAspectRatio;

        // Runtime
        [HideInInspector] public ImageData imageData;
    }
}