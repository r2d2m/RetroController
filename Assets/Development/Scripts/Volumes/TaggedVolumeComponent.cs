using UnityEngine;

public class TaggedVolumeComponent : MonoBehaviour
{
    [HideInInspector] public Material material;
    [HideInInspector] public string _tag;
    [HideInInspector] public bool isTrigger;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        gameObject.hideFlags = HideFlags.DontSave;
    }

    private void Update()
    {
        meshRenderer.material = material;
        gameObject.tag = _tag;

        if (isTrigger)
            meshCollider.convex = meshCollider.isTrigger = true;
        else
            meshCollider.convex = meshCollider.isTrigger = false;
    }
}
