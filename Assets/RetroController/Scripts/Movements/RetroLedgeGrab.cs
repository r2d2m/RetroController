﻿using UnityEngine;
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
        public bool OnLedge { get; private set; }
        RaycastHit[] raycastHits = new RaycastHit[4];

        // end point relative to the grabbing target
        Vector3 localEndPoint;
        Vector3 localGrabPoint;

        private void Awake()
        {
            OnLedge = false;
        }

        private void Update()
        {
            ClimbInput = Input.GetKey(KeyCode.Space);
        }

        #region Override
        public override bool DoMovement()
        {
            switch (movementState)
            {
                case MovementState.None:
                    if (ClimbInput && OnLedge)
                    {
                        // grab ledge
                        movementState = MovementState.Grabbing;
                        retroController.AddIgnoredCollider(GrabbingTarget);
                        localEndPoint = GrabbingTarget.transform.InverseTransformPoint(ClimbingTarget());
                        return true;
                    }
                    return false;
                case MovementState.Grabbing:
                    if (retroController.JumpInput)
                    {
                        // start climbing
                        movementState = MovementState.Climbing;
                        retroController.Velocity = Vector3.zero;
                    }
                    else
                    {
                        OnGrabbing();
                    }
                    return true;
                case MovementState.Climbing:

                    Vector3 worldEndPoint = GrabbingTarget.transform.TransformPoint(localEndPoint);
                    Vector3 nextPosition = Vector3.LerpUnclamped(
                        retroController.transform.position,
                        worldEndPoint,
                        ClimbSpeed * Time.fixedDeltaTime);

                    Vector3 diff = nextPosition - retroController.transform.position;
                    retroController.CharacterMove(diff);

                    if (Vector3.Distance(transform.position, worldEndPoint) < UnclimbDistance)
                    {
                        // finished climbing
                        Detach();
                    }
                    return true;
                default:
                    return false;
            }
        }

        public override void OnCharacterMove()
        {
            if (retroController.HasState(RetroController.CC_State.OnLadder)
                || retroController.WaterState != RetroController.CC_Water.None
                || retroController.IsGrounded
                || movementState != MovementState.None)
                return;

            if (!DetectSurfaceArea())
                return;

            if (!OnLedge)
                OnDetectLedge();
        }

        public void OnDetectLedge()
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
                        OnLedge = true;
                        GrabbingTarget = hit.collider;
                        localGrabPoint = hit.transform.InverseTransformPoint(hit.point);
                    }
                }
            }
        }

        // detect if something isn't in the way
        public void OnGrabbing()
        {
            Vector3 worldGrabPoint = GrabbingTarget.transform.TransformPoint(localGrabPoint);

            float distance = Vector3.Distance(retroController.transform.position, worldGrabPoint);
            Vector3 direction = (worldGrabPoint - retroController.transform.position).normalized;

            int n_hits = Physics.BoxCastNonAlloc(retroController.transform.position, retroController.controllerCollider.size / 2f,
                direction, raycastHits, retroController.transform.rotation, distance, 
                retroController.Profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
            
            for (int i = 0; i < n_hits; i++)
            {
                if (raycastHits[i].collider == GrabbingTarget)
                    continue;

                // something in the way, yeah...
                Detach();
                break;
            }
        }

        // Detach from grabbing
        public void Detach()
        {
            OnLedge = false;
            movementState = MovementState.None;
            retroController.ResetJumping();
            retroController.RemoveIgnoredCollider(GrabbingTarget);
            GrabbingTarget = null;
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
            
            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                RetroControllerProfile profile = retroController.Profile;
                Gizmos.DrawCube(projectedCenter, profile.Size);

                if (GrabbingTarget)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(GrabbingTarget.transform.TransformPoint(localEndPoint), 0.4f);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(GrabbingTarget.transform.TransformPoint(localGrabPoint), 0.4f);
                }
            }

        }

        public enum MovementState { None, Grabbing, Climbing }
    }

}
