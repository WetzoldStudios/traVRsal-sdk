using System;
using System.Collections.Generic;
using static traVRsal.SDK.ImageProvider;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class ModdingData
    {
        public Order imageOrder = Order.List;
        public Filter imageFilter = Filter.None;
        public bool speakNames;
        public string speakLanguage;
        public string speakVoice;
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
                    if (!id.imageLink.ToLowerInvariant().StartsWith("http")) id.imageLink = dirName + "/" + id.imageLink;
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
            imageOrder = mod.imageOrder;
            imageFilter = mod.imageFilter;
        }

        public override string ToString()
        {
            return "Modding Data";
        }
    }
}