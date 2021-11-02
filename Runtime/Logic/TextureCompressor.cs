using UnityEngine;
using UnityEngine.Rendering;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Texture Compressor")]
    public class TextureCompressor : MonoBehaviour
    {
        public bool includeInactive = true;

        private void Awake()
        {
            GetComponentsInChildren<Renderer>(includeInactive).ForEach(r => r.materials.ForEach(m =>
            {
                Shader shader = m.shader;
                for (int i = 0; i < shader.GetPropertyCount(); i++)
                {
                    if (shader.GetPropertyType(i) != ShaderPropertyType.Texture) continue;

                    Texture2D tex = (Texture2D) m.GetTexture(shader.GetPropertyName(i));
                    if (tex != null) tex.Compress(true);
                }
            }));
        }
    }
}