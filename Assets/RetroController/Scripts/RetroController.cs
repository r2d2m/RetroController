using System;
using UnityEngine;
using UnityEngine.Events;
using vnc.Utils;

namespace vnc
{
    /// <summary>
    /// The main component, here goes all the controller functionality.
    /// It's best that you inherit the class if you wanna add
    /// any features. Future updates may break your changes if you
    /// modify this class directly.
    /// </summary>
    public class RetroController : MonoBehaviour
    {
        [Header("Settings")]
        /// <summary>
        /// Set the settings for this Controller
        /// </summary>
        public RetroControllerProfile Profile;

        public Transform controllerView; // Controller view, tipically the first person camera
        public Vector3 viewPosition;     // original position for view
        
        public const float EPSILON = 0.001f;
        public const float OVERBOUNCE = 1.01f;

        private Collider[] overlapingColliders = new Collider[8];
        private Collider[] overlapOnSteps = new Collider[4];
        [HideInInspector] public CC_Collision Collisions { get; private set; }
        private BoxCollider _boxCollider;

        // Input
        [HideInInspector] public Vector2 inputDir;
        public float WalkForward { get; set; }
        public float Strafe { get; set; }
        public float Swim { get; set; }
        public bool JumpInput { get; set; }
        public bool Sprint { get; set; }
        public bool DuckInput { get; set; }

        // Velocity
        [HideInInspector] public Vector3 Velocity;
        protected Vector3 wishDir;    // the direction from the input
        protected float wishSpeed;

        // Jumping
        protected int triedJumping = 0;       // jumping timer for bunnyhopping
        protected bool wasGrounded = false;   // if player was on ground on previous update
        protected float jumpGraceTimer;       // time window for jumping just before reaching the ground
        protected bool sprintJump;            // jump while sprinting

        // Ducking
        protected float duckingTimer;
        protected bool wasDucking;

        // States
        [HideInInspector] public CC_State State { get; private set; }
        public bool IsGrounded { get { return (State & CC_State.IsGrounded) != 0; } }
        public bool OnPlatform { get { return (State & CC_State.OnPlatform) != 0; } }
        public bool OnLadder { get { return (State & CC_State.OnLadder) != 0; } }
        public bool IsDucking { get { return (State & CC_State.Ducking) != 0; } }
        public bool WalkedOnStep { get { return HasCollisionFlag(CC_Collision.CollisionStep); } }
        public bool NoClipping
        {
            get { return HasState(CC_State.NoClip);}
            set
            {
                if (value) AddState(CC_State.NoClip);
                else RemoveState(CC_State.NoClip);
            }
        }
        public SurfaceNormals surfaceNormals = new SurfaceNormals();

        // Water
        [HideInInspector] public CC_Water WaterState { get; private set; }
        public bool IsSwimming { get; private set; }
        private float waterSurfacePosY;
        private float WaterThreshold { get { return transform.position.y + Profile.SwimmingOffset; } }

        // Platforms
        protected Collider platformCollider;
        protected bool wasOnPlatform;

        // Ladders
        protected bool foundLadder = false;   // when one of the collisions found is a ladder
        protected bool detachLadder = false;  // detach from previous ladder

        // Helps camera smoothing on step.
        public float StepDelta { get; private set; }    // how much the controller went up
        [HideInInspector] public bool wasOnStep;
        public float SlopeDot { get { return (Profile.SlopeAngleLimit / 90f); } }

        [Header("Callback Events")]
        public UnityEvent OnJumpCallback;
        public UnityEvent OnLandingCallback;
        public UnityEvent OnFixedUpdateEndCallback;

        protected virtual void Awake()
        {
            State = CC_State.None;
            WaterState = CC_Water.None;
            Collisions = CC_Collision.None;
            jumpGraceTimer = Profile.JumpGraceTime;

            if (controllerView)
                viewPosition = controllerView.localPosition;

            _boxCollider = GetComponent<BoxCollider>();
            if (_boxCollider == null)
            {
                _boxCollider = gameObject.AddComponent<BoxCollider>();
            }
            _boxCollider.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }

        protected virtual void FixedUpdate()
        {
            if (Profile == null || controllerView == null)
                return;

            if (Profile.FlyingController)
            {
                FlyMovementUpdate();
            }
            else
            {
                if (OnLadder && !detachLadder)
                {
                    LadderMovementUpdate();
                }
                else if (IsSwimming && WaterState == CC_Water.Underwater)
                {
                    WaterMovementUpdate();
                }
                else
                {
                    GroundMovementUpdate();
                }
            }

            OnDuckState();

            OnFixedUpdateEndCallback.Invoke();
            wasOnStep = WalkedOnStep;
        }

        /// <summary>
        /// Entry point for feeding the controller.
        /// You can use this function to receive player
        /// inputs or AI commands.
        /// Note that the float values should range from -1 to +1. 
        /// </summary>
        /// <param name="forward">Foward input.</param>
        /// <param name="strafe">Strafe input.</param>
        /// <param name="swim">Swim input.</param>
        /// <param name="jump">Jump command.</param>
        /// <param name="sprint">Sprint command.</param>
        public virtual void SetInput(float forward, float strafe, float swim, bool jump, bool sprint, bool duck)
        {
            WalkForward = forward;
            Strafe = strafe;
            Swim = swim;
            JumpInput = jump;
            Sprint = sprint;
            DuckInput = duck;

            if (JumpInput && triedJumping == 0)
                triedJumping = Profile.JumpInputTimer;

            inputDir = new Vector2(strafe, forward);
        }

        /// <summary>
        /// Set the Ducking state on the controller
        /// </summary>
        protected virtual void OnDuckState()
        {
            if (!wasDucking && DuckInput)
                duckingTimer = Time.time;

            if (Time.time > duckingTimer + Profile.DuckingTimeDelay && DuckInput)
                AddState(CC_State.Ducking);

            if (!DuckInput && CanStand())
            {
                RemoveState(CC_State.Ducking);
                duckingTimer = 0;
            }

            wasDucking = DuckInput;
        }


        #region Movement Update
        /// <summary>
        /// Update loop for when the controller is grounded
        /// or in mid air
        /// </summary>
        protected virtual void GroundMovementUpdate()
        {
            // reset the grounded state
            if (HasCollisionFlag(CC_Collision.CollisionBelow))
                AddState(CC_State.IsGrounded);
            else
                RemoveState(CC_State.IsGrounded);

            // check platform
            if (!IsGrounded)
                RemoveState(CC_State.OnPlatform);

            jumpGraceTimer = Mathf.Clamp(jumpGraceTimer - 1, 0, Profile.JumpGraceTime);

            var walk = inputDir.y * transform.TransformDirection(Vector3.forward);
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);
            wishDir = (walk + strafe);
            wishSpeed = wishDir.magnitude;

            if (wasGrounded)
            {
                wishDir.Normalize();
                if (IsDucking)
                    Velocity = MoveGroundDucking(wishDir, Velocity);
                else
                    Velocity = MoveGround(wishDir, Velocity);
            }
            else
            {
                MoveAir();
            }

            CheckLanding();

            // bunnyhopping
            if (triedJumping > 0)
            {
                // normal jump, it's on the ground
                if (IsGrounded || jumpGraceTimer > 0)
                {
                    Velocity.y += Profile.JumpSpeed;
                    triedJumping = 0;
                    jumpGraceTimer = 0;
                    sprintJump = Sprint;
                    RemoveState(CC_State.IsGrounded);
                    OnJumpCallback.Invoke();
                }
            }

            if (!IsGrounded)
                CalculateGravity();

            // stick on platform
            if (OnPlatform)
            {
                OnPlatformMove();
            }
            else
            {
                // movement on static surface

                // was on platform?
                if (wasOnPlatform && !OnPlatform)
                {
                    platformCollider = null;
                }

                //LastCollision = p_Controller.Move(Velocity);
                CharacterMove(Velocity);
            }

            // player just got off ground
            if (wasGrounded && !IsGrounded)
            {
                // falling
                if (Velocity.normalized.y < 0)
                    jumpGraceTimer = Profile.JumpGraceTime;
            }

            triedJumping = Mathf.Clamp(triedJumping - 1, 0, 100);

            wasGrounded = IsGrounded;
            wasOnPlatform = OnPlatform;
        }

        /// <summary>
        /// Movement method when the controller is on a platform.
        /// Override it to add custom platform code.
        /// </summary>
        protected virtual void OnPlatformMove()
        {
            CharacterMove(Velocity);
        }

        /// <summary>
        /// Update loop for when the controller is underwater
        /// </summary>
        protected virtual void WaterMovementUpdate()
        {
            // player moved the character
            var walk = inputDir.y * controllerView.forward;
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);
            wishDir = (walk + strafe) + (Vector3.up * Swim);
            wishDir.Normalize();

            triedJumping = 0;   // ignores jumping on water

            Velocity = MoveWater(wishDir, Velocity);
            CalculateGravity(Profile.WaterGravityScale);

            CharacterMove(Velocity);
        }

        protected virtual void FlyMovementUpdate()
        {
            RemoveState(CC_State.IsGrounded);

            // player moved the character
            var walk = inputDir.y * controllerView.transform.forward;
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);
            wishDir = (walk + strafe) + (Vector3.up * Swim);
            wishDir.Normalize();

            // fall when dead
            Velocity = MoveFly(wishDir, Velocity);

            CharacterMove(Velocity);
        }

        /// <summary>
        /// Update loop for when the controller is attached
        /// to a ladder
        /// </summary>
        protected virtual void LadderMovementUpdate()
        {
            if (HasCollisionFlag(CC_Collision.CollisionBelow))
                AddState(CC_State.IsGrounded);
            else
                RemoveState(CC_State.IsGrounded);

            wishDir = AlignOnLadder();
            Velocity = MoveLadder(wishDir, Velocity);

            if (triedJumping > 0)
            {
                // detach and jump away from ladder
                Velocity = surfaceNormals.ladder * Profile.LadderDetachJumpSpeed;
                triedJumping = 0;
                detachLadder = true;
            }

            CharacterMove(Velocity);

            wasGrounded = IsGrounded;
        }

        /// <summary>
        /// Align the input direction alongside the ladder plane
        /// </summary>
        /// <param name="direction">The input direction</param>
        /// <returns>Vector aligned with the ladder</returns>
        protected virtual Vector3 AlignOnLadder()
        {
            var forward = inputDir.y * controllerView.forward;
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);

            // Calculate player wish direction
            Vector3 dir = forward + strafe;

            var perp = Vector3.Cross(Vector3.up, surfaceNormals.ladder);
            perp.Normalize();
            // Perpendicular in the ladder plane
            var climbDirection = Vector3.Cross(surfaceNormals.ladder, perp);

            var dNormal = Vector3.Dot(dir, surfaceNormals.ladder);
            var cross = surfaceNormals.ladder * dNormal;
            var lateral = dir - cross;

            var newDir = lateral + -dNormal * climbDirection;
            if (IsGrounded && dNormal > 0)
            {
                newDir = surfaceNormals.ladder;
            }

            return newDir;
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
            Velocity += (Vector3.down * Profile.Gravity * gravityMultiplier) * Time.fixedDeltaTime;

            // limit the Y Velocity so the player doesn't speed
            // too much when falling or being propelled
            Velocity.y = Mathf.Clamp(Velocity.y, -Profile.MaxVerticalSpeedScale, Profile.MaxVerticalSpeedScale);
        }

        protected virtual void CheckLanding()
        {
            // Apply friction when player hits the ground
            if (!wasGrounded && IsGrounded)
            {
                Vector3 vel = Friction(Velocity, Profile.GroundFriction);
                Velocity.x = vel.x;
                Velocity.z = vel.z;
                //Velocity.y = 0.001f * Math.Abs(Velocity.y);

                jumpGraceTimer = 0;
                sprintJump = false;
                OnLandingCallback.Invoke(); // notify when player reaches the gorund
            }
        }
        #endregion

        #region Movement Calculation
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
        /// Movement on the ground but while ducking.
        /// </summary>
        protected virtual Vector3 MoveGroundDucking(Vector3 wishDir, Vector3 prevVelocity)
        {
            prevVelocity = Friction(prevVelocity, Profile.GroundFriction); // ground friction
            prevVelocity = Accelerate(this.wishDir, prevVelocity, Profile.DuckingAcceleration, Profile.MaxDuckingSpeed);
            return prevVelocity;
        }

        protected virtual Vector3 MoveLadder(Vector3 wishdir, Vector3 prevVelocity)
        {
            prevVelocity = wishdir * Profile.LadderSpeed;
            return prevVelocity;
        }

        /// <summary>
        /// Movement on the air.
        /// </summary>
        protected virtual void MoveAir()
        {
            float maxSpeed = sprintJump ? Profile.MaxAirSprintSpeed : Profile.MaxAirSpeed;
            AccelerateAir(wishDir, wishSpeed, Profile.AirAcceleration, maxSpeed);
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
            // TODO: flying 
            //prevVelocity = AccelerateAir(accelDir, Profile.AirAcceleration, Profile.MaxAirSpeed);
            return prevVelocity;
        }
        #endregion Acceleration

        #region Acceleration
        protected virtual Vector3 Accelerate(Vector3 wishdir, Vector3 prevVelocity, float accelerate, float max_velocity)
        {
            var projVel = Vector3.Dot(prevVelocity, wishdir);
            float accelSpeed = accelerate;

            if (projVel + accelSpeed > max_velocity)
                accelSpeed = max_velocity - projVel;

            Vector3 newVel = prevVelocity + wishdir * accelSpeed;
            return newVel;
        }

        protected virtual void AccelerateAir(Vector3 wishDir, float wishSpeed, float accelerate, float maxSpeed)
        {
            switch (Profile.AirControl)
            {
                case AirControl.AirStrafing:
                    if (wishSpeed > maxSpeed)
                    {
                        float scale = maxSpeed / wishSpeed;
                        wishDir *= scale;
                        wishSpeed = maxSpeed;
                    }

                    float wishspd = wishDir.magnitude;
                    if (wishspd > Profile.MaxAirControl)
                        wishspd = Profile.MaxAirControl;

                    var currentSpeed = Vector3.Dot(Velocity, wishDir);
                    var addspeed = wishspd - currentSpeed;
                    if (addspeed <= 0)
                        return;

                    var accelSpeed = accelerate * wishSpeed * Time.deltaTime;
                    if (accelSpeed > addspeed)
                        accelSpeed = addspeed;

                    Velocity += accelSpeed * wishDir;
                    break;
                case AirControl.Full:
                default:
                    var projVel = Vector3.Dot(Velocity, wishDir);

                    float accelVel = accelerate * Time.fixedDeltaTime;

                    if (projVel + accelVel > maxSpeed)
                        accelVel = maxSpeed - projVel;

                    Velocity = Velocity + wishDir * accelVel;
                    break;
            }
        }
        #endregion

        #region Friction
        /// <summary>
        /// Generic friction, decrease the Velocity of the controller.
        /// </summary>
        protected virtual Vector3 Friction(Vector3 prevVelocity, float friction)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                //float control = speed < Profile.MinimumSpeed ? Profile.MinimumSpeed : speed;
                float drop = speed * friction * Time.fixedDeltaTime;

                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the Velocity based on friction.
            }
            return wishspeed;
        }

        protected virtual Vector3 WaterFriction(Vector3 prevVelocity)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                float drop = speed * Profile.WaterFriction * Time.fixedDeltaTime;
                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the Velocity based on friction.
            }
            return wishspeed;
        }
        #endregion

        #region Physics
        /// <summary>
        /// Moves the controller and calculates collision.
        /// </summary>
        /// <param name="movement">Final movement</param>
        protected virtual void CharacterMove(Vector3 movement)
        {
            SetDuckHull();

            movement = VectorFixer(movement);

            // reset all collision flags
            Collisions = CC_Collision.None;
            IsSwimming = false;

            Vector3 direction = movement.normalized;
            float distance = FloatFixer(movement.magnitude);

            if(NoClipping)
            {
                transform.position += movement;
                return;
            }

            StepDelta = Mathf.Clamp(StepDelta - Time.fixedDeltaTime, 0, Mathf.Infinity);

            if (distance > 0)
            {
                transform.position = FixOverlaps(transform.position + movement, direction, distance);
            }
            else
            {
                // when controller doesn't move
                transform.position = FixOverlaps(transform.position, Vector3.zero, 0f);
            }

            SetWaterLevel();

            // extra check to detect ground
            if (!(HasCollisionFlag(CC_Collision.CollisionBelow)))
                DetectGround();
        }

        /// <summary>
        /// Move the transform trying to stop being overlaping other colliders
        /// </summary>
        /// <param name="position">start position. Bottom of the collider</param>
        /// <returns>Final position</returns>
        protected virtual Vector3 FixOverlaps(Vector3 position, Vector3 direction, float distance)
        {
            Vector3 movement = VectorFixer(direction * distance);
            Vector3 normal;

            float dist, dot;
            dist = dot = 0f;

            // TODO: what about alive? disabling? entities?

            foundLadder = false;
            int nColls = OverlapBoxNonAlloc(movement, overlapingColliders, Profile.SurfaceLayers, QueryTriggerInteraction.Collide);

            for (int i = 0; i < nColls; i++)
            {
                Collider c = overlapingColliders[i];

                if (c.isTrigger)
                {
                    if (c.CompareTag(Profile.WaterTag))
                        CheckWater(c);
                }
                else
                {
                    //position = MoveOnSteps(position, movement);

                    if (Physics.ComputePenetration(_boxCollider, position, Quaternion.identity,
                        c, c.transform.position, c.transform.rotation, out normal, out dist))
                    {
                        // if this occur, it's a bug in the PhysX engine
                        if (float.IsNaN(normal.x) || float.IsNaN(normal.y) || float.IsNaN(normal.y))
                            continue;

                        // adjust floating point imprecision
                        dist = (float)Math.Round(dist, 3, MidpointRounding.ToEven);
                        normal = VectorFixer(normal);

                        dist += Profile.Depenetration;

                        dot = Vector3.Dot(normal, Vector3.up);

                        // COLLISIONS BELOW
                        if (dot > SlopeDot && dot <= 1)
                        {
                            Collisions = Collisions | CC_Collision.CollisionBelow;
                            position += Vector3.up * dist;
                            surfaceNormals.floor = normal;

                            if (c.CompareTag(Profile.PlatformTag))
                            {
                                // on a platform
                                // send the platform message that the player collided
                                State |= CC_State.OnPlatform;
                                platformCollider = c;
                            }
                            else
                            {
                                State &= ~CC_State.OnPlatform;
                            }

                            OnCCHit(normal);
                        }

                        // COLLISIONS ON SIDES
                        if (dot >= 0 && dot < SlopeDot)
                        {
                            Collisions = Collisions | CC_Collision.CollisionSides;

                            if (c.CompareTag(Profile.LadderTag))
                            {
                                foundLadder = true;

                                // pick the first normal on contact
                                if (!OnLadder)
                                    surfaceNormals.ladder = normal;

                            }
                            else
                            {
                                bool foundStep = false;
                                position = MoveOnSteps(position, direction, out foundStep);
                                if (!foundStep)
                                {
                                    position += normal * dist;
                                    surfaceNormals.wall = normal;
                                    OnCCHit(normal);
                                    WaterEdgePush(normal);
                                }
                            }
                        }

                        // COLLISIONS ABOVE
                        if (dot < -0.001)
                        {
                            Collisions = Collisions | CC_Collision.CollisionAbove;
                            position += normal * dist;
                            OnCCHit(normal);
                        }
                    }
                }

            }

            if (foundLadder) State |= CC_State.OnLadder;
            else
            {
                State &= ~CC_State.OnLadder;
                detachLadder = false;
            }

            return position;
        }

        #region Water
        /// <summary>
        /// Detect water surface.
        /// </summary>
        protected virtual void CheckWater(Collider waterCollider)
        {
            IsSwimming = true;

            // cast a ray from the sky and detect the topmost point
            var ray = new Ray(transform.position + Vector3.up * 1000f, Vector3.down);
            RaycastHit hit;
            if (waterCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                waterSurfacePosY = hit.point.y;
            }
        }

        /// <summary>
        /// Calculate the water level
        /// </summary>
        protected virtual void SetWaterLevel()
        {
            if (IsSwimming)
            {
                if (WaterThreshold > waterSurfacePosY)
                    WaterState = CC_Water.Partial;
                else
                    WaterState = CC_Water.Underwater;
            }
            else
            {
                WaterState = CC_Water.None;
            }
        }

        protected virtual void WaterEdgePush(Vector3 normal)
        {
            Vector3 horizontalVel = wishDir;
            horizontalVel.y = 0;

            // Check if the player is in the border of the water, give it a little push
            if (HasCollisionFlag(CC_Collision.CollisionSides)
                && Swim > 0
                && WaterState == CC_Water.Partial
                && Vector3.Dot(normal, horizontalVel) < -0.8f)
            {
                if (!Physics.Raycast(controllerView.position, horizontalVel, 1.5f, Profile.SurfaceLayers))
                {
                    Velocity.y = Profile.WaterEdgeJumpSpeed;
                }
            }
        }
        #endregion

        /// <summary>
        /// Check for steps on the way and adjust the controller.
        /// </summary>
        /// <param name="position">Next position.</param>
        /// <param name="foundStep">If a step was found.</param>
        /// <returns>Adjusted position with the possible step.</returns>
        protected virtual Vector3 MoveOnSteps(Vector3 position, Vector3 direction, out bool foundStep)
        {
            // after finding a collision, try to check if it's a step
            // the controller can be on

            foundStep = false;
            if (!IsGrounded)
            {
                return position;
            }

            RaycastHit stepHit;
            Vector3 upCenter, downCenter;
            Vector3 center, halfExtends;
            Quaternion rot;
            _boxCollider.ToWorldSpaceBox(out center, out halfExtends, out rot);
            // override center with desired position
            center = _boxCollider.center + position + (direction * 0.01f);
            // increase hull size
            halfExtends += Vector3.one * 0.01f;

            // cast up
            bool foundUp;
            foundUp = Physics.BoxCast(center, halfExtends, Vector3.up, out stepHit, Quaternion.identity, Profile.StepOffset,
                Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);

            if (foundUp && stepHit.distance > 0)
            {
                upCenter = center + (Vector3.up * stepHit.distance);
            }
            else
            {
                upCenter = center + (Vector3.up * Profile.StepOffset);
            }

            // check if it's free
            halfExtends -= Vector3.one * 0.01f;
            int nColls = Physics.OverlapBoxNonAlloc(upCenter, halfExtends, overlapOnSteps, Quaternion.identity,
                Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            if (nColls > 0)
            {
                // still overlapping something, cancel stepping
                return position;
            }

            // cast down
            bool foundDown;
            halfExtends += Vector3.one * 0.01f;
            foundDown = Physics.BoxCast(upCenter, halfExtends, Vector3.down, out stepHit, Quaternion.identity, Profile.StepOffset,
                Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            if (foundDown && stepHit.distance > 0)
            {
                downCenter = upCenter + (Vector3.down * stepHit.distance);
                if (downCenter.y > center.y)
                {
                    foundStep = true;
                    float upDist = Mathf.Abs(downCenter.y - center.y);
                    StepDelta = upDist;
                    position.y += upDist;
                    Collisions |= CC_Collision.CollisionStep;
                }
            }

            return position;
        }

        /// <summary>
        /// Controls the collision area of the capsule collider with 
        /// the Ducking state
        /// </summary>
        protected virtual void SetDuckHull()
        {
            if (IsDucking)
            {
                float t = 1;
                if (Profile.DuckingLerp)
                {
                    t = Profile.DuckingLerpSpeed * Time.fixedDeltaTime;
                }

                _boxCollider.size = Vector3.Lerp(_boxCollider.size, Profile.DuckingSize, t);
                _boxCollider.center = Vector3.Lerp(_boxCollider.center, Profile.DuckingCenter, t);

                controllerView.localPosition = Vector3.Lerp(controllerView.localPosition,
                    viewPosition + (Vector3.down * Profile.DuckingViewOffset), t);
            }
            else
            {
                _boxUpdate();
                controllerView.localPosition = viewPosition;
            }
        }

        /// <summary>
        /// Detect if there is a solid surface blocking the controller 
        /// from standing up
        /// </summary>
        /// <returns>If the collider in standing mode is free.</returns>
        protected virtual bool CanStand()
        {
            // calculate if the standing capsule won't collider with anything
            Vector3 halfExtends, duckingCenter;
            Quaternion rotation;
            var center = transform.TransformPoint(Profile.Center);
            duckingCenter = transform.TransformPoint(Profile.DuckingCenter);
            _boxCollider.ToWorldSpaceBox(out duckingCenter, out halfExtends, out rotation);
            bool isBlocking = Physics.CheckBox(center, halfExtends, Quaternion.identity, Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            return !isBlocking;
        }

        public virtual void DetectGround()
        {
            Vector3 normal;
            float distance;

            var offset = (Vector3.down * Profile.GroundCheck);

            int nColls = OverlapBoxNonAlloc(offset, overlapingColliders, Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < nColls; i++)
            {
                Collider c = overlapingColliders[i];
                var position = transform.position + offset;
                if (Physics.ComputePenetration(_boxCollider, position, transform.rotation,
                        c, c.transform.position, c.transform.rotation, out normal, out distance))
                {
                    float dot = Vector3.Dot(normal, Vector3.up);
                    float slopeDot = (Profile.SlopeAngleLimit / 90f);
                    if (dot > slopeDot && dot <= 1)
                    {
                        Collisions = Collisions | CC_Collision.CollisionBelow;
                        surfaceNormals.floor = normal;
                        OnCCHit(normal);
                    }
                }
            }
        }

        public bool HasCollisionFlag(CC_Collision flag)
        {
            return (Collisions & flag) != 0;
        }
        
        /// <summary>
        /// Called when hitting surfaces.
        /// </summary>
        /// <param name="normal">Surface normal.</param>
        protected virtual void OnCCHit(Vector3 normal)
        {
            Velocity = RetroPhysics.ClipVelocity(Velocity, normal, overbounce: true);
        }

        #endregion

        #region Utility
        /// <summary>
        /// Prevent Velocity values from losing too
        /// much precision
        /// </summary>
        /// <param name="vel">Current Velocity</param>
        /// <returns>Fixed Velocity</returns>
        private Vector3 VectorFixer(Vector3 vel)
        {
            for (int i = 0; i < 3; i++)
            {
                vel[i] = FloatFixer(vel[i]);
                if (vel[i] > -EPSILON && vel[i] < EPSILON)
                    vel[i] = 0f;
            }
            return vel;
        }

        /// <summary>
        /// Increase value precision
        /// </summary>
        /// <param name="value">Current value</param>
        /// <returns></returns>
        private float FloatFixer(float value)
        {
            return (float)Math.Round(value, 3, MidpointRounding.ToEven);
        }

        public int OverlapBoxNonAlloc(Vector3 offset, Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 center, halfExtents;
            Quaternion orientation;
            _boxCollider.ToWorldSpaceBox(out center, out halfExtents, out orientation);
            center += offset;
            return Physics.OverlapBoxNonAlloc(center, halfExtents, results, Quaternion.identity, layerMask, queryTriggerInteraction);
        }
        #region State
        public bool HasState(CC_State state)
        {
            return (State & state) != 0;
        }

        public void AddState(CC_State state)
        {
            State |= state;
        }

        public void RemoveState(CC_State state)
        {
            State &= ~state;
        }
        #endregion

        // do not modify
        protected virtual void _boxUpdate()
        {
            _boxCollider.size = Profile.Size;
            _boxCollider.center = Profile.Center;
        }

        #endregion

        #region Enums
        [Flags]
        public enum CC_State
        {
            None = 0,
            IsGrounded = 2,
            OnPlatform = 4,
            OnLadder = 8,
            Ducking = 16,
            NoClip = 32
        }

        [Flags]
        public enum CC_Collision
        {
            None = 0,
            CollisionAbove = 2,
            CollisionBelow = 4,
            CollisionSides = 8,
            CollisionStep = 16
        }

        public enum CC_Water
        {
            None,       // off water surfaces
            Partial,    // body on water, face outside
            Underwater  // submerged
        }
        #endregion

        #region Debug
        protected virtual void OnDrawGizmos()
        {
            if (Profile)
            {
                Gizmos.color = new Color(1, 0.56f, 0.06f);
                Vector3 center = transform.position + Profile.Center;
                Gizmos.DrawCube(center, Profile.Size);

                DebugExtension.DrawCircle(transform.position + Vector3.up * Profile.SwimmingOffset, Color.blue, 1f);

                DebugExtension.DrawArrow(transform.position, wishDir, Color.black);
                DebugExtension.DrawArrow(transform.position, Velocity.normalized, Color.red);

            }
        }

        #endregion
    }
}

public struct SurfaceNormals
{
    public Vector3 floor;
    public Vector3 ladder;
    public Vector3 wall;
}
