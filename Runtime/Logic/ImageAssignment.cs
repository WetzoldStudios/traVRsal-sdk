using UnityEngine;

namespace traVRsal.SDK
{
    [RequireComponent(typeof(Renderer))]
    public class ImageAssignment : ExecutorConfig
    {
        public enum ImageSource
        {
            Variable, Name
        }

        public int materialIndex = 0;
        public ImageSource source = ImageSource.Variable;
        public string variable;
    }
}