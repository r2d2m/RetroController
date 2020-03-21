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
            string text = "RC Velocity: " + retroController.Velocity +
                "\nRb Velocity: " + retroController.ControllerRigidbody.velocity;
            GUI.Label(GuiSize, text);
        }
    }

}
