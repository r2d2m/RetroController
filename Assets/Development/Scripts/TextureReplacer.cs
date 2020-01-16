using NaughtyAttributes;
using UnityEngine;

namespace vnc.Development
{
    public class TextureReplacer : MonoBehaviour
    {
        public string sourceMaterialName = "Default_Map";
        public Material target;


        [Button("Replace Materials")]
        public void Replace()
        {
            var meshRenderers = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i].material.name.StartsWith(sourceMaterialName))
                {
                    meshRenderers[i].material = target;
                }

            }
        }

    }

}
