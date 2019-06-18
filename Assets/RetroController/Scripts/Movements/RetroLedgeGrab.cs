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

        private Vector3 projectedCenter;
        private Collider[] overlapingColliders = new Collider[4];
        [EditDisabled] public bool isClimbing; // controls climbing process

        public override bool DoMovement()
        {
            if (retroController.HasState(RetroController.CC_State.OnLedge)
                && !retroController.HasState(RetroController.CC_State.OnLadder))
            {
                // start climbing
                if (retroController.JumpInput && !isClimbing)
                {
                    isClimbing = true;
                }
                else if (isClimbing)
                {
                    retroController.transform.position = Vector3.Lerp(retroController.transform.position, projectedCenter, ClimbSpeed * Time.fixedDeltaTime);

                    if (Vector3.Distance(transform.position, projectedCenter) < UnclimbDistance)
                    {
                        retroController.RemoveState(RetroController.CC_State.OnLedge);
                        isClimbing = false;
                        retroController.ResetJumping();
                    }
                }


                return true;
            }

            return false;
        }

        public override void OnCharacterMove()
        {
            if (!isClimbing)
            {
                // detect possible ledges
                RetroControllerProfile profile = retroController.Profile;

                Vector3 halfExtents;
                Quaternion orientation;
                retroController.controllerCollider.ToWorldSpaceBox(out projectedCenter, out halfExtents, out orientation);
                projectedCenter += (transform.forward * ForwardOffset) + (Vector3.up * (UpOffset + halfExtents.y));
                // check if the area is free from overlapping
                int n_overlap = Physics.OverlapBoxNonAlloc(projectedCenter, halfExtents,
                    overlapingColliders, retroController.controllerCollider.transform.rotation, profile.SurfaceLayers, QueryTriggerInteraction.Ignore);
                if (n_overlap == 0)
                {
                    //Vector3 offsetCenter = projectedCenter + (Vector3.down * Profile.GroundCheck);
                    RaycastHit hit;
                    if (Physics.BoxCast(projectedCenter, halfExtents, Vector3.down, out hit, orientation,
                        profile.GroundCheck, profile.SurfaceLayers, QueryTriggerInteraction.Ignore))
                    {
                        float dot = Vector3.Dot(hit.normal, Vector3.up);
                        float slopeDot = (profile.SlopeAngleLimit / 90f);
                        if (dot > slopeDot && dot <= 1)
                        {
                            retroController.AddState(RetroController.CC_State.OnLedge);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            DebugExtension.DrawCircle(transform.position + Vector3.up * UpOffset, Color.green, 1.2f);

            if (Application.isPlaying)
            {
                RetroControllerProfile profile = retroController.Profile;
                Gizmos.DrawCube(projectedCenter, profile.Size);
            }
        }
    }
}
