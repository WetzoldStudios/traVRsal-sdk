using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class ImageProvider
    {
        public enum Provider
        {
            Unsplash = 0,
            Device = 1
        }

        public enum Order
        {
            Random = 0,
            Newest = 1
        }

        public string key;
        public Provider provider = Provider.Unsplash;
        public Order order = Order.Random;
        public bool repeatWhenDepleted = true;

        // Formulas
        public string providerFormula;

        [HideInInspector] [NonSerialized] public object catalog;
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