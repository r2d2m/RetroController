#if UNITY_EDITOR
using epiplon.Samples;
using epiplon.Utils;
using UnityEngine;

namespace epiplon.Development
{
    [RequireComponent(typeof(RetroController))]
    public class SamplePlayerDebugger : SamplePlayer
    {
        [Header("Auto Input System")]
        public bool autoInput = false;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoFoward = 0f;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoStrafe = 0f;
        [ConditionalHide("autoInput"), Range(-1, 1)]
        public float autoSwim = 0f;
        [ConditionalHide("autoInput"), Range(-10, 10)]
        public float YRotate = 0f;
        [ConditionalHide("autoInput")] public bool autoJump = false;
        [ConditionalHide("autoInput")] public bool autoSprint = false;
        [ConditionalHide("autoInput")] public bool autoDuck = false;
        [ConditionalHide("autoInput")] public bool ignoreMouse = false;

        public InputLog[] inputLog = new InputLog[6000];
        int index = 0;
        [HideInInspector] public Vector3 recordOrigin;
        [HideInInspector] public int logCount = 0;
        [HideInInspector] public bool isRecording = false;
        [HideInInspector] public bool isPlaying = false;
        int playIndex = 0;
        float timeScale = 1f;
        
        [Header("Debug GUI Style")]
        public Vector2 guiSize;
        public GUIStyle guiStyle;

        private void Awake()
        {
            retroController = GetComponent<RetroController>();
        }

        public override void Update()
        {
            if (isPlaying)
            {
                if (playIndex < logCount)
                {
                    fwd = inputLog[playIndex].forward;
                    strafe = inputLog[playIndex].strafe;
                    swim = inputLog[playIndex].swim;
                    jump = inputLog[playIndex].jump;
                    sprint = inputLog[playIndex].sprint;
                    duck = inputLog[playIndex].duck;
                    transform.rotation = inputLog[playIndex].rotation;
                    playIndex++;
                    retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);
                }
            }
            else
            {
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
                    //fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
                    //strafe = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
                    fwd = Input.GetAxisRaw("Vertical");
                    strafe = Input.GetAxisRaw("Horizontal");

                    swim = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.C) ? 1 : 0);
                    jump = Input.GetKeyDown(KeyCode.Space);
                    sprint = Input.GetKey(KeyCode.LeftShift);
                    duck = Input.GetKey(KeyCode.C);
                    timeScale += (Input.GetKeyDown(KeyCode.KeypadPlus) ? 0.1f : 0f) - (Input.GetKeyDown(KeyCode.KeypadMinus) ? 0.1f : 0f);
                    //retroLedgeGrab.ClimbInput = Input.GetKey(KeyCode.Space);
                }

                // these inputs are fed into the controller
                // this is the main entry point for the controller
                retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);

                if (mouseLook)
                {
                    if (!(autoInput && ignoreMouse))
                    {
                        // controls mouse look
                        mouseLook.LookRotation();
                        mouseLook.UpdateCursorLock();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && mouseLook)
            {
                mouseLook.SetCursorLock(!mouseLook.lockCursor);
            }
        }
    }

    public struct InputLog
    {
        public float forward;
        public float strafe;
        public float swim;
        public bool jump;
        public bool sprint;
        public bool duck;
        public Quaternion rotation;
    }
}
#endif