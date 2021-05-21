using UnityEngine;

namespace vnc.Development
{
    [RequireComponent(typeof(RetroController))]
    public class GeneralDebugger : MonoBehaviour
    {
        public Rect GuiSize = new Rect(0, 0, 200, 60);

        RetroController retroController;
        Collider[] overlapingColliders = new Collider[4];
        bool isColliding;

        private void Awake()
        {
            retroController = GetComponent<RetroController>();
        }

        private void FixedUpdate()
        {
            float boxColliderBottom = retroController.controllerCollider.center.y - retroController.controllerCollider.size.y / 2f;
            float colliderProfileButtom = retroController.Profile.Center.y - retroController.Profile.Size.y / 2f;
            float diff = Mathf.Abs(boxColliderBottom - colliderProfileButtom) + 0.001f;

            Vector3 center = retroController.FixedPosition + retroController.Profile.Center + (transform.up * diff);
            var size = (retroController.Profile.Size / 2f);
            DebugExtension.DrawBox(center, size, transform.rotation, Color.magenta);
            isColliding = Physics.OverlapBoxNonAlloc(center, size, overlapingColliders, transform.rotation, retroController.Profile.SurfaceLayers, QueryTriggerInteraction.Ignore) > 0;
        }

        private void OnGUI()
        {
            string[] debug = new string[] {
                "Velocity: " + retroController.Velocity,
            };

            overlapingColliders.ToString();

            string text = string.Empty;
            for (int i = 0; i < debug.Length; i++)
                text += debug[i] + "\n";

            GUI.Label(GuiSize, text);
        }

        private void OnDrawGizmos()
        {

        }
    }

}
