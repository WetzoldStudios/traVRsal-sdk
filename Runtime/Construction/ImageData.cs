using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ImageData
    {
        [Header("Metadata")] public string imageLink;
        public string name;
        public string description;
        public DateTime date;
        public string author;
        public string authorLink;
        public int ratingCount;

        [Header("Runtime Data")] [NonSerialized]
        public int originalIndex;

        [NonSerialized] public int index;

        [NonSerialized] public ImageProvider config;
        [NonSerialized] public Texture2D texture;
        [NonSerialized] public object originalData;

        public ImageData()
        {
        }

        public ImageData(string imageLink, string name = null) : this()
        {
            this.imageLink = imageLink;
            this.name = name;
        }

        public ImageData Merge(ImageData data)
        {
            if (data == null) return this;

            if (!string.IsNullOrEmpty(data.author)) author = data.author;
            if (!string.IsNullOrEmpty(data.authorLink)) authorLink = data.authorLink;
            if (!string.IsNullOrEmpty(data.description)) description = data.description;
            if (!string.IsNullOrEmpty(data.name)) name = data.name;
            if (data.ratingCount > 0) ratingCount = data.ratingCount;

            return this;
        }

        public override string ToString()
        {
            return $"Image Data ({name})";
        }
    }
}