using UnityEngine;
using vnc;

namespace vnc.Movements
{
    public class RetroCrouchSlide : RetroMovement
    {
        bool sliding;
        public float speedBoost = 0.1f;
        public float stopSpeed = 0.1f;
        [Range(-1, 1)]
        public float dotLimit = 0.7f;

        public override bool DoMovement()
        {
            if (sliding)
            {
                // reset the grounded state
                if (retroController.HasCollisionFlag(RetroController.CC_Collision.CollisionBelow))
                    retroController.AddState(RetroController.CC_State.IsGrounded);
                else
                    retroController.RemoveState(RetroController.CC_State.IsGrounded);

                sliding = retroController.Velocity.magnitude > stopSpeed;
                if (!retroController.IsGrounded)
                    retroController.AddGravity();
                retroController.CharacterMove(retroController.Velocity);

                if(!sliding)
                    retroController.RemoveState(RetroController.CC_State.Ducking);

                return true;
            }
            else if (retroController.Sprint && retroController.DuckInput)
            {
                var d = Vector3.Dot(retroController.Velocity.normalized, transform.forward);
                if (d >= dotLimit)
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

        public override void OnCharacterMove()
        {

        }
    }
}
