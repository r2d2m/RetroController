using UnityEngine;

namespace vnc.Movements
{
    public class RetroCrouchSlide : RetroMovement
    {
        bool sliding;
        public float speedBoost = 0.1f;
        public float slideFriction = 0.1f;
        public float stopSpeed = 0.1f;
        [Range(-1, 1)]
        public float dotLimit = 0.7f;

        public override bool DoMovement()
        {
            if (sliding)
            {
                if (retroController.JumpInput)
                {
                    sliding = false;
                    return false;
                }

                // reset the grounded state
                if (retroController.HasCollision(RetroController.CC_Collision.CollisionBelow))
                    retroController.AddState(RetroController.CC_State.IsGrounded);
                else
                    retroController.RemoveState(RetroController.CC_State.IsGrounded);

                if (!retroController.IsGrounded)
                    retroController.AddGravity();

                retroController.Velocity = MoveSlide(retroController.Velocity);

                retroController.CharacterMove(retroController.Velocity);

                if(sliding)
                    sliding = retroController.Velocity.magnitude > stopSpeed;

                return true;
            }
            else
            {
                var d = Vector3.Dot(retroController.Velocity.normalized, transform.forward);
                if (retroController.Sprint 
                    && !retroController.HasState(RetroController.CC_State.Ducking)
                    && retroController.HasState(RetroController.CC_State.IsGrounded)
                    && retroController.DuckInput
                    && (d >= dotLimit))
                {
                    sliding = true;
                    retroController.Velocity += transform.forward * speedBoost;
                    retroController.CharacterMove(retroController.Velocity);
                    retroController.AddState(RetroController.CC_State.Ducking);
                    return true;
                }
            }

            return false;
        }

        private Vector3 MoveSlide(Vector3 prevVelocity)
        {
            prevVelocity = retroController.Friction(prevVelocity, slideFriction);
            return prevVelocity;
        }

        public override void OnCharacterMove()
        {
            if (sliding)
            {
                if (retroController.HasCollision(RetroController.CC_Collision.CollisionSides)
                    || retroController.HasCollision(RetroController.CC_Collision.CollisionStep))
                {
                    sliding = false;
                }
            }
        }
    }
}
