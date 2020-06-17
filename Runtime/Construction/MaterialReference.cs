using System;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class MaterialReference
    {
        public Renderer renderer;
        public int slot = 0;
        public Material material;

        public MaterialReference() { }

        public MaterialReference(Renderer renderer, int slot, Material material)
        {
            this.renderer = renderer;
            this.slot = slot;
            this.material = material;
        }
    }
}