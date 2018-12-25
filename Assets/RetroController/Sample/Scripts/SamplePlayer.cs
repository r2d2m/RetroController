using UnityEngine;
using vnc.Utils;

namespace vnc.Samples
{
    public class SamplePlayer : MonoBehaviour
    {
        public RetroController retroController;
        public UnityMouseLook mouseLook;
        public Camera playerCamera;
        public Animator gunAnimator;

        [Header("Input Testing")]
        public bool autoInput = false;
        [ConditionalHide("autoInput")]
        public float autoFoward = 0f;
        [ConditionalHide("autoInput")]
        public float autoStrafe = 0f;
        [ConditionalHide("autoInput")]
        public float autoSwim = 0f;
        [ConditionalHide("autoInput")]
        public bool autoJump = false;
        [ConditionalHide("autoInput")]
        public bool autoSprint = false;
        [ConditionalHide("autoInput")]
        public bool autoDuck = false;
        [ConditionalHide("autoInput")]
        public bool ignoreMouse = false;

        private void Awake()
        {
            mouseLook.Init(transform, playerCamera.transform);
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
            }
            else
            {
                // Here the sample gets input from the player
                fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
                strafe = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
                swim = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftControl) ? 1 : 0);
                jump = Input.GetKeyDown(KeyCode.Space);
                sprint = Input.GetKey(KeyCode.LeftShift);
                duck = Input.GetKey(KeyCode.LeftControl);
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
                mouseLook.LookRotation(transform, playerCamera.transform);
                mouseLook.UpdateCursorLock();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
                mouseLook.SetCursorLock(true);

            if (Input.GetKeyDown(KeyCode.Escape))
                mouseLook.SetCursorLock(false);
        }
    }
}
