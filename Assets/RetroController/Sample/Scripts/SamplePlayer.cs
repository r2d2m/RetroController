using UnityEngine;

namespace vnc.Samples
{
    /// <summary>
    /// This is a sample class to illustrate how you 
    /// give input commands to the controller.
    /// 
    /// This class is supposed to be a learning example and
    /// it's not recommended to be used in a final product.
    /// </summary>
    public class SamplePlayer : MonoBehaviour
    {
        public RetroController retroController; // the controller used
        public MouseLook mouseLook;             // mouse look
        public Transform playerView;            // the controller view
        public Animator gunAnimator;            // sample animator for the gun

        private void Awake()
        {
            mouseLook.Init(transform, playerView);
            mouseLook.SetCursorLock(true);
        }

        // input variables
        [HideInInspector] public float fwd, strafe, swim;
        [HideInInspector] public bool jump, sprint, duck;

        public virtual void Update()
        {
            // Here the sample gets input from the player
            fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
            strafe = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            swim = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.C) ? 1 : 0);
            jump = Input.GetKeyDown(KeyCode.Space);
            sprint = Input.GetKey(KeyCode.LeftShift);
            duck = Input.GetKey(KeyCode.C);

            // these inputs are fed into the controller
            // this is the main entry point for the controller
            retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);

            bool isShooting = Input.GetMouseButton(0);
            gunAnimator.SetBool("Shoot", isShooting);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mouseLook.SetCursorLock(!mouseLook.lockCursor);
            }

            mouseLook.LookRotation(transform, playerView);
            mouseLook.UpdateCursorLock();
        }



        private void OnGUI()
        {
            float width = 250;
            float height = 70;
            Rect rect = new Rect(Screen.width - width, 0, width, height);
            string label = "Mouse Sensitivity: " + mouseLook.mouseSensitivity + "\n(unlock cursor and left-click to slide";
            GUI.Label(rect, label, retroController.guiStyle);
            rect.y += 35;
            mouseLook.mouseSensitivity = GUI.HorizontalSlider(rect, mouseLook.mouseSensitivity, 0, 10);
        }
    }


}
