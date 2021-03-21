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

        [Tooltip("Specifies which material should be altered with the new texture.")]
        public int materialIndex;

        [Tooltip("Defines if a random image or the next sequentially should be used.")]
        public bool random;

        [Tooltip("Specifies a sequential order inside a zone in which images should be assigned. Typically set from the outside through a property.")]
        public int order;

        public ImageSource source = ImageSource.Name;
        public string key;

        [Tooltip("Specifies if the height of the image should be adjusted to align to the aspect ratio of the loaded image. For this to work properly the original object here should have square dimensions.")]
        public bool adjustAspectRatio;

        // Runtime
        [HideInInspector] public ImageData imageData;
    }
}