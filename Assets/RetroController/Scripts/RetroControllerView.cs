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

        public Transform controllerCamera;
        public Transform playerView;

        Vector3 cameraPosition;

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

            if (controllerCamera == null)
                Debug.LogWarning("Nothing set up for Player View.");
            else
            {
                roll.currentAngle = controllerCamera.localEulerAngles.z;
                _controller.OnJumpCallback.AddListener(() => _controller.StepCount = 0);
            }

            if (playerView == null)
                Debug.LogWarning("Nothing set up for Player View.");

            cameraPosition = playerView.position;
        }

        private void LateUpdate()
        {
            ViewUpdate();
        }

        protected virtual void ViewUpdate()
        {
            if (controllerCamera != null
                && playerView != null)
            {
                if (bob.enabled)
                    Bobbing();

                if (roll.enabled)
                    Rolling();

                StepInteporlate();
            }
        }

        #region Methods
        /// <summary>
        /// Head bobbing effect
        /// </summary>
        public virtual void Bobbing()
        {
            float bobOscillate = Mathf.Sin(bob.cycle * Mathf.Deg2Rad) / 2;

            if (_controller.updateController)
            {
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
            }

            playerView.localPosition = _controller.localViewPosition + bob.currentPosition;
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
                if (_controller.StepCount > 0)
                {
                    cameraPosition.x = playerView.position.x;
                    cameraPosition.z = playerView.position.z;

                    float speed = _controller.Sprint ? stepInterpolation.sprintSpeed : stepInterpolation.normalSpeed;
                    float t = speed * Time.deltaTime;

                    if (playerView.position.y > cameraPosition.y)
                    {
                        cameraPosition.y = Mathf.SmoothStep(cameraPosition.y, playerView.position.y, t);
                    }
                    else
                    {
                        cameraPosition.y = playerView.position.y;
                    }
                }
                else
                {
                    cameraPosition = playerView.position;
                }

                // if reaches the final position, end the step count
                if (cameraPosition == playerView.position)
                    _controller.StepCount = 0;


                controllerCamera.position = cameraPosition;
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

        public void Enabled() { enabled = true; }
        public void Disabled() { enabled = false; }
    }

}
