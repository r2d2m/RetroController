using UnityEngine;
using vnc.Utils;

namespace vnc
{
    [CreateAssetMenu(fileName = "My Controller Profile", menuName = "Retro Controller/New Controller Profile")]
    public class RetroControllerProfile : ScriptableObject
    {
        #region Gravity
        /// <summary>
        /// Default base gravity value
        /// </summary>
        [FancyHeader("Gravity")]
        public float Gravity;
        /// <summary>
        /// Gravity scale when underwater, relative to the Gravity value
        /// </summary>
        public float WaterGravityScale;
        #endregion

        #region Jumping
        [FancyHeader("Jumping")]
        /// <summary>
        /// Speed on the jump
        /// </summary>
        public float JumpSpeed;
        /// <summary>
        /// Let the character jump even after disconnecting from the group
        /// </summary>
        [Tooltip("Ledge forgiveness")]
        public int JumpGraceTime = 5;
        /// <summary>
        /// Timer window for the player to hit the jump button
        /// and make the controller jump before it reaches the 
        /// ground, so you can control how precise the player 
        /// must be to bunnyhop and gain mommentum.
        /// </summary>
        public int JumpInputTimer = 2;

        /// <summary>
        /// The speed when the controller is near the edge of a 
        /// water surface, so it can get out of it.
        /// </summary>
        public float WaterEdgeJumpSpeed;

        /// <summary>
        /// The speed applied when player jumps from the ladder
        /// </summary>
        public float LadderDetachJumpSpeed;
        #endregion

        #region Ducking
        [FancyHeader("Ducking")]
        /// <summary>
        /// Delay between issuing duck command and 
        /// getting in the ducked state
        /// </summary>
        public float DuckingTimeDelay = 0.3f;
        /// <summary>
        /// Controller center when ducking
        /// </summary>
        public Vector3 DuckingCenter = new Vector3(0f, -.5f, 0f);
        /// <summary>
        /// Controller size when ducking
        /// </summary>
        public Vector3 DuckingSize = Vector3.one;
        /// <summary>
        /// View offset based on initial view position on
        /// the controller axis orientation
        /// </summary>
        public float DuckingViewOffset = 0.8f;
        /// <summary>
        /// Enable lerping of the collider values
        /// when ducking
        /// </summary>
        public bool DuckingLerp = true;
        /// <summary>
        /// Transition speed of the controller collider
        /// size from standing to ducking
        /// </summary>
        [ConditionalHide("DuckingLerp")]
        public float DuckingLerpSpeed = 6;

        #endregion

        #region Max Speed
        [FancyHeader("Speed")]
        /// <summary>
        /// Max speed on ground
        /// </summary>
        public float MaxGroundSpeed;
        /// <summary>
        /// Max speed when running on ground.
        /// </summary>
        public float MaxGroundSprintSpeed;
        /// <summary>
        /// Max speed while on air.
        /// </summary>
        public float MaxAirSpeed;
        /// <summary>
        /// Max speed on air, after jumping while sprinting.
        /// </summary>
        public float MaxAirSprintSpeed;
        /// <summary>
        /// Max speed when underwater.
        /// </summary>
        public float MaxWaterSpeed;
        /// <summary>
        /// Max speed when ducking.
        /// Only applicable while on ground.
        /// </summary>
        public float MaxDuckingSpeed;
        /// <summary>
        /// Max absolute speed on the Y axis (limits postive and negative values)
        /// </summary>
        public float MaxVerticalSpeedScale;
        /// <summary>
        /// Speed when on a ladder.
        /// </summary>
        public float LadderSpeed;
        /// <summary>
        /// Speed on the opposite direction of the velocity.
        /// Adding a little value allows for some airborne control.
        /// Setting to 0 makes it stop completely.
        /// Don't set this value to lower than 0;
        /// </summary>
        [RangeNoSlider(0f, Mathf.Infinity)]
        public float MaxAirControl = 0.02f;
        #endregion

        #region Acceleration
        [FancyHeader("Acceleration")]
        /// <summary>
        /// Normal Acceleration on ground.
        /// </summary>
        public float GroundAcceleration;
        /// <summary>
        /// Acceleration while ducking on ground.
        /// </summary>
        public float DuckingAcceleration;
        /// <summary>
        /// Acceleration while on mid-air.
        /// </summary>
        public float AirAcceleration;
        /// <summary>
        /// Acceleration while underwater.
        /// </summary>
        public float WaterAcceleration;
        /// <summary>
        /// Define wheather the character can
        /// be controlled on mid-air (change the velocity)
        /// </summary>
        public AirControl AirControl = AirControl.Full;
        #endregion

        #region Friction
        [FancyHeader("Friction")]
        /// <summary>
        /// Ground friction, decrease speed on surfaces.
        /// </summary>
        public float GroundFriction;
        /// <summary>
        /// Friction underwater.
        /// </summary>
        public float WaterFriction;
        /// <summary>
        /// Friction for flying characters.
        /// </summary>
        public float FlyFriction;
        #endregion

        #region Controller
        [FancyHeader("Controller Properties")]
        /// <summary>
        /// Defines if a character naturally flies.
        /// These are not affected by gravity. 
        /// </summary>
        public bool FlyingController = false;        // flying characters are not affected by gravity
        /// <summary>
        /// Tolerance angle for slopes. If the angle is bigger than
        /// this value, the controller will slide
        /// </summary>
        [Range(1, 90)] public float SlopeAngleLimit = 48f;
        /// <summary>
        /// Uses the center of the controller to set a threshold for
        /// swiming status. Check the manual for a more graphical explanation.
        /// </summary>
        public float SwimmingOffset = 0.0f;
        public float StepOffset = 5f;               // maximum step height
        public Vector3 Center;
        public Vector3 Size = Vector3.one;
        #endregion

        [FancyHeader("Collision Properties")]
        /// <summary>
        /// Define which layers will the controller collide with.
        /// This include solid surfaces, platforms and water.
        /// </summary>
        public LayerMask SurfaceLayers;

        /// <summary>
        /// Distance in the Y axis to check if there is a ground
        /// </summary>
        public float GroundCheck = 0.21f;

        /// <summary>
        /// Advanced settings. Do not change this, except
        /// when experiencing collision imprecision.
        /// </summary>
        [HideInInspector] public float Depenetration = 0.001f;
        
        // TAGS
        /// <summary>
        /// Hidden property, used in <code>RetroControllerProfileEditor</code>
        /// </summary>
        [HideInInspector] public string PlatformTag;
        /// <summary>
        /// Hidden property, used in <code>RetroControllerProfileEditor</code>
        /// </summary>
        [HideInInspector] public string WaterTag;
        /// <summary>
        /// Hidden property, used in <code>RetroControllerProfileEditor</code>
        /// </summary>
        [HideInInspector] public string LadderTag;
    }

    public enum ControllerDirection { X, Y, Z };
    public enum AirControl { Full, AirStrafing }
}
