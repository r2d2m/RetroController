using UnityEngine;

namespace vnc.Samples
{
    public class SamplePlayer : MonoBehaviour
    {
        public RetroController retroController;
        public UnityMouseLook mouseLook;
        public Camera playerCamera;
        public Animator gunAnimator;

        private void Awake()
        {
            mouseLook.Init(transform, playerCamera.transform);
        }

        void Update()
        {
            // Here the sample gets input from the player
            float fwd = (Input.GetButton("Forward") ? 1 : 0) - (Input.GetButton("Backwards") ? 1 : 0);
            float strafe = (Input.GetButton("Strafe_Right") ? 1 : 0) - (Input.GetButton("Strafe_Left") ? 1 : 0);
            float swim = Input.GetAxisRaw("Swim");
            bool jump = Input.GetButtonDown("Jump");
            bool sprint = Input.GetButton("Sprint");

            // these inputs are fed into the controller
            // this is the main entry point for the controller
            retroController.SetInput(fwd, strafe, swim, jump, sprint);

            // animation for the sample
            bool isShooting = Input.GetButton("Fire1");
            gunAnimator.SetBool("Shoot", isShooting);

            // controls mouse look
            mouseLook.LookRotation(transform, playerCamera.transform);
            mouseLook.UpdateCursorLock();

            if (Input.GetKeyDown(KeyCode.Mouse0))
                mouseLook.SetCursorLock(true);

            if (Input.GetKeyDown(KeyCode.Escape))
                mouseLook.SetCursorLock(false);
        }
    }
}
