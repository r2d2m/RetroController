using System;
using UnityEngine;

namespace vnc.Movements
{
    public class RetroSwing : RetroMovement
    {
        SwingState swingState = SwingState.None;
        Collider grapplePointCollider;
        Vector3 direction;

        public float pullSpeed = 1f; // pull to right position
        public float hookLength = 5f;
        public float swingStrength = .1f;

        public Transform mainView;
        public LineRenderer lineRenderer;

        float forceTime = 0;
        public override bool DoMovement()
        {
            bool keyDown = Input.GetKeyDown(KeyCode.Mouse1);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            if (jump)
            {
                RemoveState(SwingState.Grabbed);
                RemoveState(SwingState.Swinging);
            }

            if (HasState(SwingState.Grabbed))
            {
                var distance = Vector3.Distance(grapplePointCollider.transform.position, retroController.FixedPosition);
                if (distance > hookLength)
                {
                    var direction = (grapplePointCollider.transform.position - retroController.FixedPosition).normalized;
                    retroController.Velocity = direction * pullSpeed;
                }
                else
                {
                    retroController.Velocity = direction * swingStrength;
                    retroController.AddGravity();

                }

                retroController.CharacterMove(retroController.Velocity);


                lineRenderer.SetPositions(new[] { grapplePointCollider.transform.position, retroController.FixedPosition });
            }
            else
            {
                if (keyDown)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(retroController.transform.position, mainView.forward, out hit, 1000, 1 << 0, QueryTriggerInteraction.Ignore))
                    {
                        Debug.Log("Grabbed");
                        grapplePointCollider = hit.collider;
                        lineRenderer.SetPositions(new[] { grapplePointCollider.transform.position, retroController.FixedPosition });
                        AddState(SwingState.Grabbed);
                    }
                }
            }

            return HasState(SwingState.Grabbed);
        }

        public override void OnCharacterMove()
        {
        }

        #region State Utils
        public bool HasState(SwingState state)
        {
            return (swingState & state) != 0;
        }

        public void AddState(SwingState state)
        {
            swingState |= state;
        }

        public void RemoveState(SwingState state)
        {
            swingState &= ~state;
        }
        #endregion

        [Flags]
        public enum SwingState
        {
            None = 0,
            Grabbed = 1,
            Swinging = 2
        }
    }

}
