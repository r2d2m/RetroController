using UnityEngine;

namespace vnc.Movements
{
    public class RetroLadder : RetroMovement
    {
        // small attach force for continuous ladder detection
        public float attachForce = 0.1f;

        bool foundLadder, onLadder;
        bool detach;    

        public override bool DoMovement()
        {
            if (onLadder)
            {
                retroController.RemoveState(RetroController.CC_State.Ducking);
                if (retroController.HasCollision(RetroController.CC_Collision.CollisionBelow))
                    retroController.AddState(RetroController.CC_State.IsGrounded);
                else
                    retroController.RemoveState(RetroController.CC_State.IsGrounded);

                var wishDir = MoveOnLadder();
                retroController.Velocity = wishDir * retroController.Profile.LadderSpeed;

                if (retroController.TriedJumping > 0)
                {
                    // detach and jump away from ladder
                    retroController.Velocity = retroController.surfaceNormals.sides * retroController.Profile.LadderDetachJumpSpeed;
                    retroController.ResetJumping();
                    detach = true;
                    onLadder = false;
                }

                retroController.CharacterMove(retroController.Velocity);
                retroController.WasGrounded = retroController.IsGrounded;
                foundLadder = false;
                return true;
            }

            return false;
        }

        Vector3 MoveOnLadder()
        {
            var inputDir = retroController.inputDir;
            var controllerView = retroController.controllerView;
            var gravityDirection = retroController.gravityDirection;
            var ladderNormal = retroController.surfaceNormals.sides;

            var forward = inputDir.y * controllerView.forward;
            var strafe = inputDir.x * transform.TransformDirection(Vector3.right);

            // Calculate player wish direction
            Vector3 dir = forward + strafe;

            var perp = Vector3.Cross(gravityDirection, ladderNormal);
            perp.Normalize();
            // Perpendicular in the ladder plane
            var climbDirection = Vector3.Cross(ladderNormal, perp);

            var dNormal = Vector3.Dot(dir, ladderNormal);
            var cross = ladderNormal * dNormal;
            var lateral = dir - cross;

            var newDir = lateral + -dNormal * climbDirection;
            if (retroController.IsGrounded && dNormal > 0)
            {
                newDir = ladderNormal;
            }
            else
            {
                newDir -= ladderNormal * attachForce;
            }

            return newDir;
        }

        public override void OnCollisionSide(Collider collider)
        {
            if (collider.tag == retroController.Profile.LadderTag)
                foundLadder = true;
        }

        public override void OnCharacterMove()
        {
            // ignores immediate collision with ladder on detaching
            if (detach)
            {
                detach = false;
                return;
            }

            onLadder = foundLadder;
        }
    }
}
