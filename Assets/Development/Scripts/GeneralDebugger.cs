using UnityEngine;

namespace vnc.Development
{
    [RequireComponent(typeof(RetroController))]
    public class GeneralDebugger : MonoBehaviour {

        RetroController retroController;
        public Rect GuiSize = new Rect(0, 0, 200, 60);

        private void Awake()
        {
            retroController = GetComponent<RetroController>();
        }

        private void OnGUI()
        {
            string text = "Collisions: " + retroController.Collisions;
            GUI.Label(GuiSize, text);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Vector3 center = retroController.FixedPosition + retroController.Profile.Center;
                Gizmos.DrawWireCube(center, retroController.Profile.Size);
            }
        }
    }

}
