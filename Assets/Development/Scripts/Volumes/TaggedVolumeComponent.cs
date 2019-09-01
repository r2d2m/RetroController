using UnityEngine;

namespace vnc.Development
{
    public class TaggedVolumeComponent : MonoBehaviour
    {
        [HideInInspector] public Material material;
        [HideInInspector] public string _tag;
        [HideInInspector] public bool isTrigger;
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshCollider meshCollider;
        
        private void Update()
        {
            gameObject.tag = _tag;

            if (meshRenderer)
                meshRenderer.material = material;

            if(meshCollider)
            {
                if (isTrigger)
                    meshCollider.convex = meshCollider.isTrigger = true;
                else
                    meshCollider.convex = meshCollider.isTrigger = false;
            }
        }
    }
}
