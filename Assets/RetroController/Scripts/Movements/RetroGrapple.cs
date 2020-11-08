using System;
using UnityEngine;
using UnityEngine.Events;
using vnc.Utils;

namespace vnc.Movements
{
    public class RetroGrapple : ExperimentalRetroMovement
    {
        GrappleState grappleState = GrappleState.None;
        Vector3 grapplePoint;
        Vector3 direction;

        public float pullSpeed = 0.1f;
        public float gravityForce = 1f;
        public float airFriction = 1f;
        public float maxHookDistance = 80f;
        public float airControlSpeed = 1f;
        public float minHookLenght = 3f;
        float hookLenght;

        public Transform mainView;
        public LineRenderer lineRenderer;

        public override bool DoMovement()
        {
            bool keyDown = Input.GetKeyDown(KeyCode.Mouse1);
            bool jumpKeyDown = Input.GetKeyDown(KeyCode.Space);

            if (jumpKeyDown)
            {
                RemoveState(GrappleState.Pull);
            }

            if (keyDown)
            {
                RaycastHit hit;
                if (Physics.Raycast(retroController.FixedPosition, mainView.forward, out hit, maxHookDistance, 1 << 0))
                {
                    grapplePoint = hit.point;
                    hookLenght = Vector3.Distance(grapplePoint, retroController.FixedPosition);
                    AddState(GrappleState.Pull);
                }
            }

            if (HasState(GrappleState.Pull))
            {
                var inputDir = retroController.inputDir;
                var strafe = inputDir.x * retroController.transform.TransformDirection(Vector3.right);

                var distance = Vector3.Distance(grapplePoint, retroController.FixedPosition);
                direction = (grapplePoint - retroController.FixedPosition).normalized;

                if(distance >= minHookLenght)
                {
                    var pull = (distance / hookLenght) * pullSpeed;
                    retroController.Velocity += direction * pull;
                }

                retroController.Velocity = retroController.Accelerate(strafe, retroController.Velocity, airControlSpeed, RetroProfile.MaxAirSpeed);
                retroController.Velocity = retroController.Friction(retroController.Velocity, airFriction);
                retroController.AddGravity(gravityForce);
                retroController.CharacterMove(retroController.Velocity);
            }

            return HasState(GrappleState.Pull);
        }

        public override void OnCharacterMove()
        {
        }

        private void Update()
        {
            if (HasState(GrappleState.Pull))
            {
                lineRenderer.SetPositions(new[] { grapplePoint, retroController.FixedPosition });
            }
        }

        #region State Utils
        public bool HasState(GrappleState state)
        {
            return (grappleState & state) != 0;
        }

        public void AddState(GrappleState state)
        {
            grappleState |= state;
        }

        public void RemoveState(GrappleState state)
        {
            grappleState &= ~state;
        }
        #endregion

        [Flags]
        public enum GrappleState
        {
            None = 0,
            LaunchBall = 1,
            Pull = 2
        }
    }

    public class HookEvent : UnityEvent<Vector3> { }
}
