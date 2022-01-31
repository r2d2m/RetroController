using UnityEngine;

namespace epiplon.Movements
{
    public class RetroWallRun : RetroMovement
    {
        public State state = State.None;
        float speed;
        Vector3 wallRunDirection;

        public override bool DoMovement()
        {
            switch (state)
            {
                case State.WallRunning:
                    retroController.Velocity = wallRunDirection * speed;
                    retroController.CharacterMove(retroController.Velocity);
                    // TODO: check for threshold to get out of wall running state
                    return true;
                case State.CanWallRun:
                    // TODO: prompt for jump input
                    state = State.WallRunning;
                    return false;
                case State.None:
                default:
                    if (retroController.Collisions == RetroController.CC_Collision.CollisionSides
                && !retroController.IsGrounded)
                    {
                        state = State.CanWallRun;
                        var xzVelocity = retroController.Velocity;
                        xzVelocity.y = 0;
                        wallRunDirection = Vector3.ProjectOnPlane(xzVelocity.normalized, retroController.surfaceNormals.sides);
                        speed = xzVelocity.magnitude;
                    }
                    return false;
            }
        }

        public override void OnCharacterMove()
        {

        }

        private void OnGUI()
        {
            var r = new Rect(0, 0, 200, 50);
            GUI.Label(r, "Can Attach: " + state);
            r.y += 30;
            GUI.Label(r, "Direction: " + wallRunDirection);
            r.y += 30;
            GUI.Label(r, "Collisions: " + retroController.Collisions);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + wallRunDirection * 5);
        }

        public enum State
        {
            None,
            CanWallRun,
            WallRunning
        }
    }
}
