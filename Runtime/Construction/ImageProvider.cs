using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ImageProvider
    {
        public enum Provider
        {
            Unsplash
        }

        public enum Order
        {
            Random = 0,
            Newest = 1
        }

        public string key;
        public Provider provider = Provider.Unsplash;
        public Order order = Order.Random;

        [HideInInspector]
        public object catalog;
        [HideInInspector]
        public bool fetchingCatalog = false;
        [HideInInspector]
        public int currentIndex = 0;

        public ImageProvider() { }

        public override string ToString()
        {
            return $"Image provider ({provider})";
        }
    }
}