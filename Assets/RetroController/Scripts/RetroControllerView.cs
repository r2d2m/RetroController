using UnityEngine;
using vnc.Utils;

namespace vnc
{
    /// <summary>
    /// The controller 
    /// </summary>
    [RequireComponent(typeof(RetroController))]
    public class RetroControllerView : MonoBehaviour
    {
        RetroController _controller;
        public Transform controllerCamera;

        [Header("Settings")]
        public Bob bob;
        public Roll roll;
        public Step stepInterpolation;

        float horizontalVelocityMagnitude
        {
            get
            {
                var velocity = _controller.Velocity;
                velocity.y = 0f;
                return velocity.magnitude;
            }
        }

        private void Awake()
        {
            _controller = GetComponent<RetroController>();
            roll.currentAngle = controllerCamera.localEulerAngles.z;
            _controller.OnFixedUpdateEndCallback.AddListener(ViewUpdate);
        }

        void ViewUpdate()
        {
            if (bob.enabled)
                Bobbing();

            if (roll.enabled)
                Rolling();

            StepInteporlate();
        }

        #region Methods
        /// <summary>
        /// Head bobbing effect
        /// </summary>
        public void Bobbing()
        {
            float bobOscillate = Mathf.Sin(bob.cycle * Mathf.Deg2Rad) / 2;

            if (bob.whenMovingOnly)
            {

                if (Mathf.Clamp01(horizontalVelocityMagnitude) > RetroController.STOP_EPSILON)
                {
                    bob.cycle += (Time.deltaTime * bob.speed);
                    if (bob.cycle >= 360) bob.cycle = 0;
                    //bobOscillate *= velocityMag;
                }
            }
            else
            {
                bob.cycle += (Time.deltaTime * bob.speed);
                if (bob.cycle >= 360) bob.cycle = 0;

            }

            bob.currentPosition = bob.origin + (bob.offset * bobOscillate);
            controllerCamera.localPosition = bob.currentPosition;
        }

        /// <summary>
        /// Roll the camera in the Z axis
        /// </summary>
        public void Rolling()
        {
            float rolltarget = -(_controller.Strafe * roll.angle);
            float step = (rolltarget - roll.currentAngle) * Time.deltaTime;

            roll.currentAngle = Mathf.Clamp(roll.currentAngle + step, -roll.angle, roll.angle);

            Vector3 angles = controllerCamera.localEulerAngles;
            angles.z = roll.currentAngle;
            controllerCamera.localEulerAngles = angles;
        }

        /// <summary>
        /// Interpolate the stepping and makes it feel smoother
        /// </summary>
        public void StepInteporlate()
        {
            if (stepInterpolation.enabled)
            {

                if (_controller.WalkedOnStep && !_controller.wasOnStep)
                {
                    Debug.Log("Added " + _controller.StepDelta + " to step delta.");
                    stepInterpolation.delta += _controller.StepDelta;
                }

                controllerCamera.localPosition = bob.currentPosition + (Vector3.down * stepInterpolation.delta);

                float speed = _controller.Sprint ? stepInterpolation.sprintSpeed : stepInterpolation.normalSpeed;
                float t = speed * Time.deltaTime;

                stepInterpolation.delta -= Easings.Interpolate(t, stepInterpolation.easingFunction);
                stepInterpolation.delta = Mathf.Clamp(stepInterpolation.delta, 0, float.MaxValue);
            }
            else
            {
                controllerCamera.localPosition = bob.currentPosition;
            }
        }
        #endregion

    }

    [System.Serializable]
    public struct Bob
    {
        public bool enabled;
        public bool whenMovingOnly;
        public Vector3 origin;
        public Vector3 offset;
        public float speed;
        [EditDisabled] public Vector3 currentPosition;
        [EditDisabled] public float cycle;
    }

    [System.Serializable]
    public struct Roll
    {
        public bool enabled;
        public float angle;
        public float speed;
        [EditDisabled] public float currentAngle;
    }

    [System.Serializable]
    public struct Step
    {
        public bool enabled;
        public float normalSpeed;
        public float sprintSpeed;
        public Easings.Functions easingFunction;
        [EditDisabled] public float delta;
        [EditDisabled] public float nextPosY;
        [EditDisabled] public float viewUpPosition;
        [EditDisabled] public bool walkedStep;
    }

}
