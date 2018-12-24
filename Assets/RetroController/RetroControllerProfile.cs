using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vnc
{
    [CreateAssetMenu(fileName = "My Controller Profile", menuName = "Retro Controller/New Controller Profile")]
    public class RetroControllerProfile : ScriptableObject
    {
        // TODO: add a Scale property to level each group (acceleration, gravity, friction, etc)

        #region Gravity
        /// <summary>
        /// Default base gravity value
        /// </summary>
        [Header("Gravity")]
        public float Gravity;
        /// <summary>
        /// Gravity scale when underwater, relative to the Gravity value
        /// </summary>
        public float WaterGravityScale;
        #endregion

        #region Jumping
        [Header("Jumping")]
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

        #region Max Speed
        [Header("Max Speed")]
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
        /// When the controller speed reaches a really small value,
        /// it stops completely.
        /// </summary>
        public float StopSpeed;
        /// <summary>
        /// Max absolute speed on the Y axis (limits postive and negative values)
        /// </summary>
        public float MaxVerticalSpeedScale;
        /// <summary>
        /// Speed when on a ladder.
        /// </summary>
        public float LadderSpeed;
        #endregion

        #region Acceleration
        [Header("Acceleration")]
        /// <summary>
        /// Scales all the acceleration properties in the profile.
        /// </summary>
        public float AccelerationScale = 1f;
        /// <summary>
        /// Normal Acceleration on ground.
        /// Used as a base.
        /// </summary>
        public float GroundAcceleration;
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
        public bool AirboneControl = false;
        #endregion

        #region Friction
        [Header("Friction")]
        /// <summary>
        /// Scales all the friction properties in the profile.
        /// </summary>
        public float FrictionScale = 1f;
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
        [Header("Controller")]
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
        public float Radius = 1;
        public float Height = 2;
        public ControllerDirection Direction = ControllerDirection.Y;
        #endregion

        [Header("Collision")]
        /// <summary>
        /// Define which layers will the controller collide with.
        /// This include solid surfaces, platforms and water.
        /// </summary>
        public LayerMask SurfaceLayers;
        /// <summary>
        /// Advanced settings. Do not change this, except
        /// when experiencing collision imprecision.
        /// </summary>
        public float Depenetration = 0.001f;

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
}
