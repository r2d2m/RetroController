using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace vnc
{
    public class RetroController : MonoBehaviour
    {
        /// <summary>
        /// Set the settings for this Controller
        /// </summary>
        public RetroControllerProfile Profile;
        public bool showDebugStats = false;

        private const float STOP_EPSILON = 0.01f;

        private Collider[] overlapingColliders = new Collider[16];
        [HideInInspector] public CC_Collision Collisions { get; private set; }
        private CapsuleCollider _capsuleCollider;

        // Input
        [HideInInspector] public Vector2 inputDir;
        public float WalkForward { get; set; }
        public float Strafe { get; set; }
        public float Swim { get; set; }
        public bool JumpInput { get; set; }
        public bool Sprint { get; set; }

        // Velocity
        private Vector3 velocity;
        public Vector3 Velocity { get { return velocity; } private set { velocity = value; } }
        private Vector3 wishdir;
        private float wishspeed;

        // Jumping
        private int triedJumping = 0;
        private bool wasGrounded = false;
        private float jumpGraceTimer;
        private bool sprintJump;

        // States
        [HideInInspector] public CC_State State { get; private set; }
        public bool IsGrounded { get { return (State & CC_State.IsGrounded) != 0; } }
        public bool OnPlatform { get { return (State & CC_State.OnPlatform) != 0; } }
        public bool IsSwimming { get; private set; }
        private float waterSurfacePosY;

        /// Helps camera smoothing on step.
        public float StepDelta { get; private set; }

        // Platforms
        private Platform currentPlatform;
        private Collider platformCollider;
        private bool wasOnPlatform;

        [Header("Events")]
        public UnityEvent OnJump;
        public UnityEvent OnLanding;

        protected virtual void Awake()
        {
            State = CC_State.None;
            jumpGraceTimer = Profile.JumpGraceTime;

            _capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            _capsuleCollider.hideFlags = HideFlags.NotEditable;
        }

        protected virtual void FixedUpdate()
        {
            if (Profile.FlyingController)
            {
                //FlyMovementUpdate();
            }
            else
            {
                if (IsSwimming)
                {
                    //WaterMovementUpdate();
                }
                else
                {
                    GroundMovementUpdate();
                }
            }
        }

        /// <summary>
        /// Entry point for feeding the controller.
        /// You can use this function to receive player
        /// inputs or AI commands.
        /// Note that the float values should range from -1 to +1. 
        /// </summary>
        /// <param name="fwd">Foward input.</param>
        /// <param name="strafe">Strafe input.</param>
        /// <param name="swim">Swim input.</param>
        /// <param name="jump">Jump command.</param>
        /// <param name="sprint">Sprint command.</param>
        public void SetInput(float fwd, float strafe, float swim, bool jump, bool sprint)
        {
            WalkForward = fwd;
            Strafe = strafe;
            Swim = swim;
            JumpInput = jump;
            Sprint = sprint;

            if (JumpInput && triedJumping == 0)
                triedJumping = Profile.JumpInputTimer;

            inputDir = new Vector2(strafe, fwd);
        }

        protected virtual void GroundMovementUpdate()
        {
            // reset the grounded state
            State = (Collisions & CC_Collision.CollisionBelow) != 0 ? State | CC_State.IsGrounded : State & ~CC_State.IsGrounded;
            if (!IsGrounded)
                State &= ~CC_State.OnPlatform;

            jumpGraceTimer = Mathf.Clamp(jumpGraceTimer - 1, 0, Profile.JumpGraceTime);

            var walk = inputDir.y * transform.TransformDirection(Vector3.forward);
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);
            wishdir = (walk + strafe);
            wishdir.Normalize();

            if (wasGrounded)
            {
                velocity = MoveGround(wishdir, velocity);
            }
            else
            {
                velocity = MoveAir(wishdir, velocity);
            }

            // bunnyhopping
            if (triedJumping > 0)
            {
                // normal jump, it's on the ground
                if (IsGrounded || jumpGraceTimer > 0)
                {
                    velocity.y = Profile.JumpSpeed;
                    triedJumping = 0;
                    jumpGraceTimer = 0;
                    sprintJump = Sprint;
                    OnJump.Invoke();
                }
            }

            CalculateGravity();

            // stick on platform
            if (OnPlatform)
            {
                if (currentPlatform == null)
                    currentPlatform = platformCollider.GetComponent<Platform>();

                if (currentPlatform)
                    CharacterMove(velocity + currentPlatform.Velocity);
            }
            else
            {
                // normal movement

                // was on platform?
                if (wasOnPlatform && !OnPlatform)
                {
                    currentPlatform = null;
                }

                //LastCollision = p_Controller.Move(velocity);
                CharacterMove(velocity);
            }

            // player just got off ground
            if (wasGrounded && !IsGrounded)
            {
                // falling
                if (velocity.normalized.y < 0)
                    jumpGraceTimer = Profile.JumpGraceTime;
            }

            triedJumping = Mathf.Clamp(triedJumping - 1, 0, 100);

            // Apply friction when player hits the ground
            if (!wasGrounded && IsGrounded)
            {
                Vector3 friction = Friction(velocity, 20);
                velocity.x = friction.x;
                velocity.z = friction.z;

                jumpGraceTimer = 0;
                sprintJump = false;
                OnLanding.Invoke(); // notify when player reaches the gorund
            }

            wasGrounded = IsGrounded;
            wasOnPlatform = OnPlatform;
        }

        /// <summary>
        /// Push the controller down the Y axis based on gravity value on settings
        /// </summary>
        /// <param name="gravityMultiplier">
        /// Multiplier used when dealing with different environments, like water. 
        /// </param>
        protected virtual void CalculateGravity(float gravityMultiplier = 1f)
        {
            // calculate gravity but on water
            velocity += (Vector3.down * Profile.Gravity * gravityMultiplier) * Time.fixedDeltaTime;

            // limit the Y velocity so the player doesn't speed
            // too much when falling or being propelled
            velocity.y = Mathf.Clamp(velocity.y, -Profile.MaxVerticalSpeedScale, Profile.MaxVerticalSpeedScale);
        }
        #region Movement
        /// <summary>
        /// Movement on the ground, most common operation.
        /// </summary>
        protected virtual Vector3 MoveGround(Vector3 wishdir, Vector3 prevVelocity)
        {
            prevVelocity = Friction(prevVelocity, Profile.GroundFriction); // ground friction
            float maxVelocity = Sprint ? Profile.MaxGroundSprintSpeed : Profile.MaxGroundSpeed;
            prevVelocity = Accelerate(wishdir, prevVelocity, Profile.GroundAcceleration, maxVelocity);
            return prevVelocity;
        }

        /// <summary>
        /// Movement on the air.
        /// </summary>
        protected virtual Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
        {
            float maxVelocity = sprintJump ? Profile.MaxAirSprintSpeed : Profile.MaxAirSpeed;
            prevVelocity = AccelerateAir(accelDir, prevVelocity, Profile.AirAcceleration, maxVelocity);
            return prevVelocity;
        }

        // move with water friction
        protected virtual Vector3 MoveWater(Vector3 accelDir, Vector3 prevVelocity)
        {
            prevVelocity = WaterFriction(prevVelocity);
            return Accelerate(accelDir, prevVelocity, Profile.WaterAcceleration, Profile.MaxWaterSpeed);
        }

        /// <summary>
        /// Default movement for flying controllers. To not confuse with "MoveAir".
        /// </summary>
        protected virtual Vector3 MoveFly(Vector3 accelDir, Vector3 prevVelocity)
        {
            prevVelocity = Friction(prevVelocity, Profile.FlyFriction);
            prevVelocity = AccelerateAir(accelDir, prevVelocity, Profile.AirAcceleration, Profile.MaxAirSpeed);
            return prevVelocity;
        }
        #endregion Acceleration

        #region Acceleration
        private Vector3 Accelerate(Vector3 wishdir, Vector3 prevVelocity, float accelerate, float max_velocity)
        {
            var projVel = Vector3.Dot(prevVelocity, wishdir);
            float accelSpeed = accelerate * Profile.AccelerationScale;

            if (projVel + accelSpeed > max_velocity)
                accelSpeed = max_velocity - projVel;

            Vector3 newVel = prevVelocity + wishdir * accelSpeed;
            return newVel;
        }

        private Vector3 AccelerateAir(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
        {
            var projVel = Vector3.Dot(prevVelocity, accelDir);
            if (Profile.AirboneControl)
            {
                float accelVel = accelerate * Profile.AccelerationScale;

                if (projVel + accelVel > max_velocity)
                    accelVel = max_velocity - projVel;

                Vector3 newVel = prevVelocity + accelDir * accelVel;
                return newVel;
            }
            else if (projVel < 0)
            {
                float accelVel = accelerate;

                if (projVel + accelVel > max_velocity)
                    accelVel = max_velocity - projVel;

                Vector3 newVel = prevVelocity + accelDir * accelVel;
                return newVel;
            }
            return prevVelocity;
        }
        #endregion

        #region Friction
        /// <summary>
        /// Generic friction, decrease the velocity of the controller.
        /// </summary>
        private Vector3 Friction(Vector3 prevVelocity, float friction)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                float control = speed < Profile.StopSpeed ? Profile.StopSpeed : speed;
                float drop = control * friction * Profile.FrictionScale * Time.fixedDeltaTime;

                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
            }
            return wishspeed;
        }

        private Vector3 WaterFriction(Vector3 prevVelocity)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                float drop = speed * Profile.WaterFriction * Profile.FrictionScale * Time.fixedDeltaTime;
                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
            }
            return wishspeed;
        }
        #endregion

        #region Enums
        [Flags]
        public enum CC_State
        {
            None = 0,
            IsGrounded = 2,
            OnPlatform = 4
        }

        [Flags]
        public enum CC_Collision
        {
            None = 0,
            CollisionAbove = 2,
            CollisionBelow = 4,
            CollisionSides = 8,
            WalkedStep = 16
        }
        #endregion

        #region Physics
        /// <summary>
        /// Moves the controller and calculates collision.
        /// </summary>
        /// <param name="movement">Final movement</param>
        protected virtual void CharacterMove(Vector3 movement)
        {
            _capsuleUpdate();
            movement = VelocityFixer(movement);

            Vector3 nTotal = Vector3.zero;

            // reset all collision flags
            Collisions = CC_Collision.None;

            Vector3 movNormalized = movement.normalized;
            float distance = movement.magnitude;

            // TODO: add option for noclip cheating
            //if (Game.Settings.Cheat_Noclip)
            //{
            //    transform.position += movement;
            //    return;
            //}

            // Calculates stairs
            StepDelta = Mathf.Clamp(StepDelta - Time.fixedDeltaTime * 1.5f, 0, Mathf.Infinity);
            if (IsGrounded && !Profile.FlyingController)
                MoveOnSteps(movNormalized);

            //const float minimumStepDistance = 0.1f; // Greater than 0 to prevent infinite loop
            //float stepDistance = Math.Min((ownCollider as CapsuleCollider).radius, minimumStepDistance);
            float stepDistance = 0.05f;

            Vector3 nResult;
            if (distance > 0)
            {
                for (float curDist = 0; curDist < distance; curDist += stepDistance)
                {
                    float curMagnitude = Math.Min(stepDistance, distance - curDist);
                    Vector3 start = transform.position;
                    Vector3 end = start + movNormalized * curMagnitude;
                    transform.position = FixOverlaps(end, movNormalized * curMagnitude, out nResult);
                    nTotal += nResult;
                }
            }
            else
            {
                // when controller doesn't move
                transform.position = FixOverlaps(transform.position, Vector3.zero, out nResult);
                nTotal += nResult;
            }

            CheckWater();

            // handles collision
            OnCCHit(nTotal.normalized);
        }

        /// <summary>
        /// Move the transform trying to stop being overlaping other colliders
        /// </summary>
        /// <param name="position">start position. Bottom of the collider</param>
        /// <returns>Final position</returns>
        Vector3 FixOverlaps(Vector3 position, Vector3 offset, out Vector3 nResult)
        {
            Vector3 nTemp = Vector3.zero;

            // TODO: what about alive? disabling? entities?

            int nColls = OverlapCapsuleNonAlloc(offset, overlapingColliders, Profile.SolidSurfaceLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < nColls; i++)
            {
                Collider c = overlapingColliders[i];

                Vector3 normal;
                float dist;
                float dot;
                if (Physics.ComputePenetration(_capsuleCollider, position, transform.rotation,
                    c, c.transform.position, c.transform.rotation, out normal, out dist))
                {
                    // TODO: report bug
                    if (float.IsNaN(normal.x) || float.IsNaN(normal.y) || float.IsNaN(normal.y))
                        continue;

                    dist += Profile.Depenetration;

                    dot = Vector3.Dot(normal, Vector3.up);
                    float slopeDot = (Profile.SlopeAngleLimit / 90f);
                    if (dot > slopeDot && dot <= 1)
                    {
                        Collisions = Collisions | CC_Collision.CollisionBelow;

                        position += Vector3.up * dist;

                        // check if platform
                        if (CompareLayer(c.gameObject, Profile.PlatformSurfaceLayers))
                        {
                            // send the platform message that the player collided
                            State |= CC_State.OnPlatform;
                            platformCollider = c;
                        }
                        else // above a regular solid
                        {
                            State &= ~CC_State.OnPlatform;
                        }
                    }
                    else
                    {
                        position += normal * dist;
                    }

                    if (dot >= 0 && dot < Profile.SlopeAngleLimit)
                    {
                        Collisions = Collisions | CC_Collision.CollisionSides;
                    }

                    if (dot < 0)
                    {
                        Collisions = Collisions | CC_Collision.CollisionAbove;
                    }



                    nTemp += normal;
                }
            }
            nResult = nTemp;
            return position;
        }

        /// <summary>
        /// Detect water surface.
        /// </summary>
        public void CheckWater()
        {
            int n_col = OverlapCapsuleNonAlloc(Vector3.zero, overlapingColliders, Profile.WaterSurfaceLayers, QueryTriggerInteraction.Collide);
            if (n_col > 0)
            {
                var col = overlapingColliders[0];
                var ray = new Ray(transform.position + Vector3.up * 1000f, Vector3.down);
                RaycastHit hit;
                if (col.Raycast(ray, out hit, Mathf.Infinity))
                {
                    waterSurfacePosY = hit.point.y;
                    float fpsPosY = transform.position.y;
                    IsSwimming = fpsPosY < waterSurfacePosY;
                }
            }
        }

        /// <summary>
        /// Check for steps on the way and adjust the controller.
        /// </summary>
        /// <param name="movNormalized">Normalized vector.</param>
        void MoveOnSteps(Vector3 movNormalized)
        {
            RaycastHit stepHit;
            RaycastHit cornerHit;
            Vector3 stepDir;
            float stepDist;
            Vector3 stepCenter;
            Vector3 p0, p1; // capsule point 0 and 1
            float radius;   // capsule radius

            // ignore gravity pull
            stepDir = movNormalized;
            stepDir.y = 0;
            if (stepDir == Vector3.zero)
                return;

            stepDist = stepDir.magnitude; // for debug purposes

            ToWorldSpaceControllerCapsule(out p0, out p1, out radius);
            p0 += (stepDir * stepDist) + (Vector3.up * Profile.StepOffset);
            p1 += (stepDir * stepDist) + (Vector3.up * Profile.StepOffset);

            if (stepDist <= Mathf.Epsilon)
                return;

            // check if collides in the next step
            if (Physics.CheckCapsule(p0, p1, radius, Profile.SolidSurfaceLayers, QueryTriggerInteraction.Ignore))
            {
                // collided with a solid object, probably a wall
                return; // doesn't do anything
            }
            else
            {
                // didn't found a collision, so there is a step
                // try to find the step point
                stepCenter = ((p0 + p1) / 2) + stepDir * radius;
                Vector3 size = new Vector3(Profile.Radius * 2, Profile.Height, Profile.Radius * 2);

                if (Physics.CapsuleCast(p0 + stepDir * radius, p1 + stepDir * radius, radius, Vector3.down, out stepHit, Mathf.Infinity, Profile.SolidSurfaceLayers, QueryTriggerInteraction.Ignore))
                {
                    var bottom = Profile.Center + transform.position + (Vector3.down * Profile.Height / 2);

                    if (Physics.Raycast(stepHit.point + Vector3.up * Profile.StepOffset,
                        Vector3.down, out cornerHit, Mathf.Infinity, Profile.SolidSurfaceLayers, QueryTriggerInteraction.Ignore))
                    {
                        var dot = Vector3.Dot(cornerHit.normal, Vector3.up);
                        if (stepHit.point.y > bottom.y && dot >= 0.98999999f)
                        {
                            float upDist = Mathf.Abs(stepHit.point.y - bottom.y);
                            transform.position += Vector3.up * upDist; //raise the player on the step size

                            if (upDist > StepDelta)
                                StepDelta = upDist;

                            Collisions |= CC_Collision.CollisionBelow;
                            Collisions |= CC_Collision.WalkedStep;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Called when hitting surfaces.
        /// </summary>
        /// <param name="normal">Surface normal.</param>
        protected virtual void OnCCHit(Vector3 normal)
        {
            // reset Y speed 
            if ((Collisions & CC_Collision.CollisionAbove) != 0 && velocity.y > 0)
            {
                velocity.y = 0;
            }

            // adjust velocity on side surfaces
            if ((Collisions & CC_Collision.CollisionSides) != 0)
            {
                var copyVelocity = velocity;
                copyVelocity.y = 0;
                var newDir = Vector3.ProjectOnPlane(copyVelocity.normalized, normal);
                velocity.x = (newDir * copyVelocity.magnitude).x;
                velocity.z = (newDir * copyVelocity.magnitude).z;
            }
        }

        #endregion

        #region Utility
        /// <summary>
        /// Prevent velocity values from losing too
        /// much precision
        /// </summary>
        /// <param name="vel">Current velocity</param>
        /// <returns>Fixed velocity</returns>
        public Vector3 VelocityFixer(Vector3 vel)
        {
            for (int i = 0; i < 3; i++)
            {
                if (vel[i] > -STOP_EPSILON && vel[i] < STOP_EPSILON)
                    vel[i] = 0f;
            }
            return vel;
        }

        public void ToWorldSpaceControllerCapsule(out Vector3 point0, out Vector3 point1, out float radius)
        {
            var center = transform.TransformPoint(Profile.Center);
            radius = 0f;
            float height = 0f;
            Vector3 lossyScale = AbsVec3(transform.lossyScale);
            Vector3 dir = Vector3.zero;

            switch (Profile.Direction)
            {
                case ControllerDirection.X:
                    radius = Mathf.Max(lossyScale.y, lossyScale.z) * Profile.Radius;
                    height = lossyScale.x * Profile.Height;
                    dir = transform.TransformDirection(Vector3.right);
                    break;
                case ControllerDirection.Y:
                    radius = Mathf.Max(lossyScale.x, lossyScale.z) * Profile.Radius;
                    height = lossyScale.y * Profile.Height;
                    dir = transform.TransformDirection(Vector3.up);
                    break;
                case ControllerDirection.Z:
                    radius = Mathf.Max(lossyScale.x, lossyScale.y) * Profile.Radius;
                    height = lossyScale.z * Profile.Height;
                    dir = transform.TransformDirection(Vector3.forward);
                    break;
            }

            if (height < radius * 2f)
            {
                dir = Vector3.zero;
            }

            point0 = center + dir * (height * 0.5f - radius);
            point1 = center - dir * (height * 0.5f - radius);
        }

        public int OverlapCapsuleNonAlloc(Vector3 offset, Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 point0, point1;
            float radius;
            ToWorldSpaceControllerCapsule(out point0, out point1, out radius);
            point0 += offset;
            point1 += offset;
            return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, queryTriggerInteraction);
        }

        public Vector3 AbsVec3(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public bool CompareLayer(GameObject obj, LayerMask layerMask)
        {
            return ((1 << obj.layer) & layerMask) != 0;
        }

        // do not modify
        private void _capsuleUpdate()
        {
            _capsuleCollider.height = Profile.Height;
            _capsuleCollider.radius = Profile.Radius;
            _capsuleCollider.center = Profile.Center;
        }

        #endregion

        #region Debug
        protected virtual void OnDrawGizmos()
        {
            if (Profile)
            {
                Vector3 start = transform.position + Profile.Center + (Vector3.up * (Profile.Height / 2f));
                Vector3 end = transform.position + Profile.Center + (Vector3.down * (Profile.Height / 2f));

                DebugExtension.DrawCapsule(start, end, Color.yellow, Profile.Radius);
            }
        }

        private void OnGUI()
        {
            if (showDebugStats)
            {
                Rect rect = new Rect(0, 0, 200, 200);
                string debugText = "Velocity: " + Velocity;
                GUI.Label(rect, debugText);
            }
        }
        #endregion
    }
}
