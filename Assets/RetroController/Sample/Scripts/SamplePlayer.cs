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


        public InputLog[] inputLog = new InputLog[6000];
        int index = 0;
        [HideInInspector] public Vector3 recordOrigin;
        [HideInInspector] public int logCount = 0;
        [HideInInspector] public bool isRecording = false;
        [HideInInspector] public bool isPlaying = false;
        int playIndex = 0;

        private void Awake()
        {
            mouseLook.Init(transform, playerView);
        }

        float fwd, strafe, swim = 0f;
        bool jump, sprint, duck = false;

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.R) && !isPlaying)
            {
                if (isRecording) StopRecording();
                else StartRecording();
            }

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
            }


            if (Input.GetKeyDown(KeyCode.Mouse0))
                mouseLook.SetCursorLock(true);

            if (Input.GetKeyDown(KeyCode.Escape))
                mouseLook.SetCursorLock(false);

            if (isRecording)
                Record();
        }

        #region Input Recording

        public void Record()
        {
            if (index < inputLog.Length - 1)
            {
                inputLog[index] = new InputLog
                {
                    forward = fwd,
                    strafe = strafe,
                    swim = swim,
                    sprint = sprint,
                    duck = duck,
                    jump = jump,
                    rotation= transform.rotation
                };
                index++;
            }
            else
            {
                Debug.Log("Input log array is full");
                StopRecording();
            }
        }

        public void StartRecording()
        {
            isRecording = true;
            recordOrigin = transform.position;
        }

        public void StopRecording()
        {
            isRecording = false;
            logCount = index + 1;
            index = 0;
        }

        public void StartPlaying()
        {
            isPlaying = true;
            playIndex = 0;
            transform.position = recordOrigin;
            retroController.Velocity = Vector3.zero;
        }

        public void StopPlaying()
        {
            isPlaying = false;
            playIndex = 0;
        }

        #endregion

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
