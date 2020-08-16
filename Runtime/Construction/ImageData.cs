using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ImageData
    {
        public Texture texture;
        public string name;
        public string description;
        public DateTime date;
        public string author;
        public string authorLink;
        public string imageLink;
        public int ratingCount;

        [Header("Runtime Data")]
        public int index;
        public ImageProvider config;

        public override string ToString()
        {
            return $"Image data ({name})";
        }
    }
}