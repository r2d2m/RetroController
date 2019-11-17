#if UNITY_EDITOR
using UnityEngine;
using vnc.Movements;
using vnc.Samples;
using vnc.Utils;
namespace vnc.Development
{
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
        float timeScale = 1f;

        [Header("Custom Movement")]
        public RetroLedgeGrab retroLedgeGrab;
        public Camera gunCamera;

        [Header("Debug GUI Style")]
        public GUIStyle guiStyle;

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && !isPlaying)
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
                    timeScale += (Input.GetKeyDown(KeyCode.KeypadPlus) ? 0.1f : 0f) - (Input.GetKeyDown(KeyCode.KeypadMinus) ? 0.1f : 0f);
                    //retroLedgeGrab.ClimbInput = Input.GetKey(KeyCode.Space);
                }

                // these inputs are fed into the controller
                // this is the main entry point for the controller
                retroController.SetInput(fwd, strafe, swim, jump, sprint, duck);


                if (!(autoInput && ignoreMouse))
                {
                    // controls mouse look
                    mouseLook.LookRotation();
                    mouseLook.UpdateCursorLock();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mouseLook.SetCursorLock(!mouseLook.lockCursor);
            }

            if (isRecording)
                Record();

            Time.timeScale = timeScale;

            if(gunCamera && retroLedgeGrab)
                gunCamera.enabled = retroLedgeGrab.movementState == RetroLedgeGrab.MovementState.None;


            DebugGUI.LogPersistent("velocity", "Velocity: " + retroController.Velocity);
            Vector2 XZ = new Vector2(retroController.Velocity.x, retroController.Velocity.z);
            DebugGUI.LogPersistent("velocity_magnitude", "XZ Magnitude: " + XZ.magnitude);
            DebugGUI.LogPersistent("onground", "Is Grounded: " + retroController.IsGrounded);

        }

        //protected virtual void OnGUI()
        //{
        //    if (Application.isEditor)
        //    {
        //        Rect rect = new Rect(0, 0, 250, 100);
        //        Vector3 planeVel = retroController.Velocity; planeVel.y = 0;
        //        string debugText = "States: " + retroController.State;

        //        if (guiStyle != null)
        //            GUI.Label(rect, debugText, guiStyle);
        //        else
        //            GUI.Label(rect, debugText);
        //    }
        //}

        private void OnDrawGizmos()
        {
            if (retroController == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (retroController.WishDirection * 3));
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
                    rotation = transform.rotation
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