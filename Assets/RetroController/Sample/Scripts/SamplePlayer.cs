using UnityEngine;
using vnc.Utils;

namespace vnc.Samples
{
    public class SamplePlayer : MonoBehaviour
    {
        public RetroController retroController;
        public UnityMouseLook mouseLook;
        public Transform playerView;
        public Animator gunAnimator;
        public Rigidbody body;

        public bool autoInput = false;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoFoward = 0f;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoStrafe = 0f;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoSwim = 0f;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float YRotate = 0f;
        [ConditionalHide("autoInput")] public bool autoJump = false;
        [ConditionalHide("autoInput")] public bool autoSprint = false;
        [ConditionalHide("autoInput")] public bool autoDuck = false;
        [ConditionalHide("autoInput")] public bool ignoreMouse = false;

        private void Awake()
        {
            mouseLook.Init(transform, playerView);
        }

        void Update()
        {
            float fwd, strafe, swim = 0f;
            bool jump, sprint, duck = false;

            if (autoInput)
            {
                fwd = autoFoward;
                strafe = autoStrafe;
                swim = autoSwim;
                jump = autoJump;
                sprint = autoSprint;
                duck = autoDuck;
                transform.rotation *= Quaternion.Euler(0, YRotate, 0);
            }
            else
            {
                // Here the sample gets input from the player
                fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
                strafe = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
                swim = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.C) ? 1 : 0);
                jump = Input.GetKeyDown(KeyCode.Space);
                sprint = Input.GetKey(KeyCode.LeftShift);
                duck = Input.GetKey(KeyCode.C);
            }

            // these inputs are fed into the controller
            // this is the main entry point for the controller
            retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);

            // animation for the sample
            bool isShooting = Input.GetMouseButton(0);
            gunAnimator.SetBool("Shoot", isShooting);

            if (!(autoInput && ignoreMouse))
            {
                // controls mouse look
                mouseLook.LookRotation(transform, playerView);
                mouseLook.UpdateCursorLock();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
                mouseLook.SetCursorLock(true);

            if (Input.GetKeyDown(KeyCode.Escape))
                mouseLook.SetCursorLock(false);

        }

        private void OnGUI()
        {
            float width = 250;
            float height = 100;
            Rect rect = new Rect(Screen.width - width, 0, width, height);
            string label = "Mouse Sensitivity: " + mouseLook.mouseSensitivity + "\n(unlock cursor and left-click to slide";
            GUI.Label(rect, label, retroController.guiStyle);
            rect.y += 35;
            mouseLook.mouseSensitivity = GUI.HorizontalSlider(rect, mouseLook.mouseSensitivity, 0, 10);
        }
    }
}
