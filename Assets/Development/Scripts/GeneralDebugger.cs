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
            GUI.Label(GuiSize, "Velocity: " + retroController.Velocity);
        }
    }

}
