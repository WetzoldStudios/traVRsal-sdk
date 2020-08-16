using UnityEngine;

namespace traVRsal.SDK
{
    [RequireComponent(typeof(Renderer))]
    public class ImageAssignment : ExecutorConfig, IDataSource
    {
        public enum ImageSource
        {
            Name = 0,
            URL = 3,
            ImageProvider = 2,
            Variable = 1
        }

        public int materialIndex = 0;
        public ImageSource source = ImageSource.Name;
        public string key;

        [HideInInspector]
        public ImageData imageData;
    }
}