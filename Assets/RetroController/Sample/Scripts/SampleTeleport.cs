using UnityEngine;

namespace vnc.Samples
{
    public class SampleTeleport : MonoBehaviour
    {
        RetroController retroController;
        public Camera cameraTarget;
        public MeshRenderer targetMeshRenderer;
        public Material sourceMaterial;

        RenderTexture renderTexture;
        Material renderMaterial;

        private void Awake()
        {
            retroController = FindObjectOfType<RetroController>();

            renderTexture = new RenderTexture(64, 64, 24, RenderTextureFormat.ARGB1555, RenderTextureReadWrite.Default);

            renderMaterial = new Material(sourceMaterial);
            renderMaterial.SetTexture("_MainTex", renderTexture);
            targetMeshRenderer.material = renderMaterial;

            cameraTarget.targetTexture = renderTexture;
            cameraTarget.gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == retroController.gameObject)
            {
                retroController.TeleportTo(cameraTarget.transform.position);
            }
        }
    }
}
