using UnityEngine;
using vnc;

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
        public bool ignoreMouse = false;

        private void Awake()
        {
            mouseLook.Init(transform, playerCamera.transform);

            retroController.OnLanding.AddListener(() => { Debug.Log("Landed"); });
        }

        void Update()
        {
            float fwd, strafe, swim = 0f;
            bool jump, sprint = false;

            if (autoInput)
            {
                fwd = autoFoward;
                strafe = autoStrafe;
                swim = autoSwim;
                jump = autoJump;
                sprint = autoSprint;
            }
            else
            {
                // Here the sample gets input from the player
                fwd = (Input.GetButton("Forward") ? 1 : 0) - (Input.GetButton("Backwards") ? 1 : 0);
                strafe = (Input.GetButton("Strafe_Right") ? 1 : 0) - (Input.GetButton("Strafe_Left") ? 1 : 0);
                swim = Input.GetAxisRaw("Swim");
                jump = Input.GetButtonDown("Jump");
                sprint = Input.GetButton("Sprint");
            }

            // these inputs are fed into the controller
            // this is the main entry point for the controller
            retroController.SetInput(fwd, strafe, swim, jump, sprint);

            // animation for the sample
            bool isShooting = Input.GetButton("Fire1");
            gunAnimator.SetBool("Shoot", isShooting);

            if (!ignoreMouse)
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
