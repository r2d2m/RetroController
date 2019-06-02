#pragma warning disable 0649
using UnityEngine;
using vnc.Utils;

namespace vnc
{
    /// <summary>
    /// The controller view.
    /// Optional but requires the main component.
    /// </summary>
    [RequireComponent(typeof(RetroController))]
    public class RetroControllerView : MonoBehaviour
    {
        protected RetroController _controller;
        [SerializeField] protected Transform controllerCamera;

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

        protected virtual void Awake()
        {
            _controller = GetComponent<RetroController>();
            roll.currentAngle = controllerCamera.localEulerAngles.z;
            _controller.OnFixedUpdateEndCallback.AddListener(ViewUpdate);
        }

        protected virtual void ViewUpdate()
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
        public virtual void Bobbing()
        {
            float bobOscillate = Mathf.Sin(bob.cycle * Mathf.Deg2Rad) / 2;

            if (bob.whenMovingOnly)
            {

                if (Mathf.Clamp01(horizontalVelocityMagnitude) > RetroController.EPSILON)
                {
                    bob.cycle += (Time.fixedDeltaTime * bob.speed);
                    if (bob.cycle >= 360) bob.cycle = 0;
                }
            }
            else
            {
                bob.cycle += (Time.fixedDeltaTime * bob.speed);
                if (bob.cycle >= 360) bob.cycle = 0;

            }

            bob.currentPosition = bob.origin + (bob.offset * bobOscillate);
            controllerCamera.localPosition = bob.currentPosition;
        }

        /// <summary>
        /// Roll the camera in the Z axis
        /// </summary>
        public virtual void Rolling()
        {
            float rolltarget = -(_controller.Strafe * roll.angle);
            float step = (rolltarget - roll.currentAngle) * roll.speed * Time.deltaTime;

            roll.currentAngle = Mathf.Clamp(roll.currentAngle + step, -roll.angle, roll.angle);

            Vector3 angles = controllerCamera.localEulerAngles;
            angles.z = roll.currentAngle;
            controllerCamera.localEulerAngles = angles;
        }

        /// <summary>
        /// Interpolate the stepping and makes it feel smoother
        /// </summary>
        public virtual void StepInteporlate()
        {
            if (stepInterpolation.enabled)
            {
                if (_controller.WalkedOnStep && !_controller.wasOnStep)
                {
                    stepInterpolation.delta += _controller.StepDelta;
                }
                
                stepInterpolation.delta = Mathf.Clamp(stepInterpolation.delta, 0, stepInterpolation.maximumDelta);
                controllerCamera.localPosition = bob.currentPosition + (Vector3.down * stepInterpolation.delta);

                float speed = _controller.Sprint ? stepInterpolation.sprintSpeed : stepInterpolation.normalSpeed;
                float t = speed * Time.deltaTime;

                stepInterpolation.delta -= Easings.Interpolate(t, stepInterpolation.easingFunction);
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
        [Tooltip("Toggle camera bobbing")]
        public bool enabled;
        public bool whenMovingOnly;
        public Vector3 origin;
        public Vector3 offset;
        public float speed;
        [EditDisabled] public Vector3 currentPosition;
        [EditDisabled] public float cycle;

        public void Enabled() { enabled = true; }
        public void Disabled() { enabled = false; }
    }

    [System.Serializable]
    public struct Roll
    {
        [Tooltip("Toggle camera roll.")]
        public bool enabled;
        [Tooltip("Maximum angle to both left and right.")]
        public float angle;
        [Tooltip("How fast it will roll.")]
        public float speed;
        [EditDisabled] public float currentAngle;

        public void Enabled() { enabled = true; }
        public void Disabled() { enabled = false; }
    }

    [System.Serializable]
    public struct Step
    {
        [Tooltip("Toggle interpolation.")]
        public bool enabled;        // enables interpolation
        public float normalSpeed;   // interpolation speed while walking
        public float sprintSpeed;   // interpolation speed while sprinting
        [RangeNoSlider(0f, float.MaxValue)]
        public float maximumDelta;  
        public Easings.Functions easingFunction;    // interpolation function to be used
        [EditDisabled] public float delta;
        [EditDisabled] public float nextPosY;
        [EditDisabled] public float viewUpPosition;
        [EditDisabled] public bool walkedStep;

        public void Enabled() { enabled = true; }
        public void Disabled() { enabled = false; }
    }

}
