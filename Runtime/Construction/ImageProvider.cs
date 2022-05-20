using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ImageProvider
    {
        public enum Provider
        {
            Unsplash = 0,
            Giphy = 1,
            Device = 2,
            World = 3
        }

        public enum Order
        {
            Native = 0,
            Random = 3,
            List = 4,
            DateAsc = 1,
            DateDesc = 2,
            NameAsc = 5,
            NameDesc = 6
        }

        public enum Filter
        {
            None = 0,
            List = 1
        }

        public string key;
        public Provider provider = Provider.Unsplash;
        public string folder;
        public Order order = Order.Random;
        public Filter filter = Filter.None;
        public bool repeatWhenDepleted = true;

        // Formulas
        public string providerFormula;

        [HideInInspector] [NonSerialized] public List<ImageData> catalog;
        [HideInInspector] [NonSerialized] public bool fetchingCatalog;
        [HideInInspector] [NonSerialized] public int currentIndex;

        public ImageProvider()
        {
        }

        public override string ToString()
        {
            return $"Image Provider ({provider})";
        }
    }
}