using UnityEngine;
using vnc.Utils;

namespace vnc.Movements
{
    public class RetroLedgeGrab : RetroMovement
    {
        public float UpOffset = 0.7f;
        public float ForwardOffset = 0.7f;
        public float ClimbSpeed = 3f;
        public float UnclimbDistance = 0.1f;
        [HideInInspector] public bool ClimbInput;
        public Vector3 contactExtension;

        private Vector3 projectedCenter;
        private Collider[] overlapingColliders = new Collider[4];
        [EditDisabled] public MovementState movementState;
        public Collider GrabbingTarget { get; private set; }
        public RaycastHit LedgeHit { get; private set; }

        private void Update()
        {
            ClimbInput = Input.GetKey(KeyCode.Space);
        }

        #region Override
        public override bool DoMovement()
        {
            DetectLedge();
            bool onLedge = retroController.HasState(RetroController.CC_State.OnLedge);

            switch (movementState)
            {
                case MovementState.None:
                    if (ClimbInput && onLedge)
                    {
                        movementState = MovementState.Grabbing;
                        retroController.AddIgnoredCollider(GrabbingTarget);
                        return true;
                    }
                    return false;
                case MovementState.Grabbing:
                    if (retroController.JumpInput)
                    {
                        movementState = MovementState.Climbing;
                        retroController.Velocity = Vector3.zero;
                    }
                    return true;
                case MovementState.Climbing:

                    Vector3 nextPosition = Vector3.LerpUnclamped(retroController.transform.position, ClimbingTarget(), ClimbSpeed * Time.fixedDeltaTime);
                    Vector3 diff = nextPosition - retroController.transform.position;
                    retroController.CharacterMove(diff);

                    if (Vector3.Distance(transform.position, ClimbingTarget()) < UnclimbDistance)
                    {
                        // finished climbing
                        retroController.RemoveState(RetroController.CC_State.OnLedge);
                        movementState = MovementState.None;
                        retroController.ResetJumping();
                        retroController.RemoveIgnoredCollider(GrabbingTarget);
                    }
                    return true;
                default:
                    return false;
            }
        }

        public override void OnCharacterMove()
        {
            if (retroController.HasState(RetroController.CC_State.OnLadder)
                || retroController.IsGrounded
                || movementState != MovementState.None)
                return;

            if (!DetectSurfaceArea())
                return;

            DetectLedge();
        }

        public void DetectLedge()
        {
            Vector3 halfExtents;
            Quaternion orientation;
            retroController.controllerCollider.ToWorldSpaceBox(out projectedCenter, out halfExtents, out orientation);
            projectedCenter += (transform.forward * ForwardOffset) + (Vector3.up * (UpOffset + halfExtents.y));

            RetroControllerProfile profile = retroController.Profile;

            // check if the area is free from overlapping
            int n_overlap = Physics.OverlapBoxNonAlloc(projectedCenter, halfExtents,
                overlapingColliders, retroController.controllerCollider.transform.rotation, profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            if (n_overlap == 0)
            {
                RaycastHit hit;
                if (Physics.BoxCast(projectedCenter, halfExtents, Vector3.down, out hit, orientation,
                    profile.GroundCheck, profile.SurfaceLayers, QueryTriggerInteraction.Ignore))
                {
                    float dot = Vector3.Dot(hit.normal, Vector3.up);
                    float slopeDot = (profile.SlopeAngleLimit / 90f);
                    if (dot > slopeDot && dot <= 1)
                    {
                        retroController.CheckPlatform(hit.collider);
                        retroController.AddState(RetroController.CC_State.OnLedge);
                        GrabbingTarget = hit.collider;
                        LedgeHit = hit;
                    }
                }
            }
        }
        #endregion

        public Vector3 ClimbingTarget()
        {
            Vector3 center, halfExtents;
            Quaternion orientation;
            retroController.controllerCollider.ToWorldSpaceBox(out center, out halfExtents, out orientation);
            center += (transform.forward * ForwardOffset) + (Vector3.up * (UpOffset + halfExtents.y));
            return center;
        }

        public bool DetectSurfaceArea()
        {
            return Physics.CheckBox(transform.position + transform.forward, contactExtension, transform.rotation,
                retroController.Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmos()
        {
            if (retroController == null)
                return;

            Gizmos.color = Color.magenta;
            DebugExtension.DrawCircle(transform.position + Vector3.up * UpOffset, Color.green, 1.2f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position + transform.forward, contactExtension);

            if (Application.isPlaying)
            {
                RetroControllerProfile profile = retroController.Profile;
                Gizmos.DrawCube(projectedCenter, profile.Size);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(ClimbingTarget(), Vector3.down);
            }

        }

        public enum MovementState { None, Grabbing, Climbing }
    }

}
