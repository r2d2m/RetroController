using System;
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

        [Space, Tooltip("Switch to ducking and standing by pressing once instead of holding")]
        public bool toggleDucking;

        [Header("Animation")]
        public Animator playerAnimator;
        public float animDelta = 6f;
        float animHorizontal, animVertical;

        private void Start()
        {
            if(mouseLook)
            {
                mouseLook.Init(retroController, playerView);
                mouseLook.SetCursorLock(true);
            }
            else
            {
                Debug.LogWarning("No MouseLook assigned.");
            }

            if (playerAnimator)
            {
                retroController.OnJumpCallback.AddListener(() =>
                {
                    playerAnimator.SetTrigger("Jump");
                });
            }
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

            // you can choose how your controller is gonna duck
            if (toggleDucking)
            {
                // switch between modes by hitting the button once
                if (Input.GetKeyDown(KeyCode.C))
                    duck = !duck;
            }
            else
            {
                // requires the player to hold the button
                duck = Input.GetKey(KeyCode.C);
            }

            // these inputs are fed into the controller
            // this is the main entry point for the controller
            retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);

            if (mouseLook)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    mouseLook.SetCursorLock(!mouseLook.lockCursor);
                    retroController.updateController = !retroController.updateController;
                }

                mouseLook.LookRotation();
                mouseLook.UpdateCursorLock();
            }

            Time.timeScale = retroController.updateController ? 1 : 0;

            SetAnimatorParameters();
        }

        private void SetAnimatorParameters()
        {
            if (playerAnimator)
            {
                float angle = playerView.eulerAngles.x;
                angle = (angle > 180) ? angle - 360 : angle;
                playerAnimator.SetFloat("Angle", angle);

                animHorizontal = Mathf.MoveTowards(animHorizontal, retroController.inputDir.x, Time.deltaTime * animDelta);
                animVertical = Mathf.MoveTowards(animVertical, retroController.inputDir.y, Time.deltaTime * animDelta);

                playerAnimator.SetFloat("Horizontal", animHorizontal);
                playerAnimator.SetFloat("Vertical", FixSmallValues(animVertical));
                playerAnimator.SetFloat("Speed", Mathf.Clamp(retroController.Velocity.magnitude, 1, 3));
                playerAnimator.SetBool("Ducking", retroController.IsDucking);
                playerAnimator.SetBool("OnGround", retroController.IsGrounded);
            }
        }

        const float SMALL = 0.01f;
        float FixSmallValues (float value)
        {
            if (value > -SMALL && value < SMALL)
                return 0;

            return value;
        }
    }

}
