using UnityEngine;

namespace epiplon.Development
{
    [RequireComponent(typeof(RetroController))]
    public class LandingTest : MonoBehaviour
    {
        RetroController retroController;

        private void Awake()
        {
            retroController = GetComponent<RetroController>();
            retroController.OnLanding.AddListener((c) => Debug.Log("Landed on " + c.name ));
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 200, 60), "Is Grounded: " + retroController.IsGrounded +
                "\nJump Grace: " + retroController.JumpGraceTimer);
        }
    }
}
