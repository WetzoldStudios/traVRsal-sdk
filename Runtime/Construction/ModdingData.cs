using System;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    [Serializable]
    public class ModdingData
    {
        public List<ImageData> imageData;

        public ModdingData()
        {
        }

        public void Merge(ModdingData mod, string dirName)
        {
            if (mod == null) return;
            if (mod.imageData != null)
            {
                mod.imageData.ForEach(id =>
                {
                    if (!id.imageLink.ToLower().StartsWith("http")) id.imageLink = dirName + "/" + id.imageLink;
                });
                if (imageData == null)
                {
                    imageData = mod.imageData;
                }
                else
                {
                    imageData.AddRange(mod.imageData);
                }
            }
        }

        public override string ToString()
        {
            return "Modding Data";
        }
    }
}