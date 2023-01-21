using UnityEngine;
using epiplon.Utils;

namespace epiplon.Movements
{
    public class RetroDoubleJump : RetroMovement
    {
        bool doubleJumped = false;

        [Tooltip("Double Jump when below vertical speed threshold?")]
        public bool allowSpeedThreshold = true;
        [ConditionalHide(ConditionalSourceField = "allowSpeedThreshold")]
        public float threshold = -0.1f;

        [Tooltip("Replace the Profile Jump Speed value?")]
        public bool customJumpSpeed = false;
        [ConditionalHide(ConditionalSourceField = "customJumpSpeed")]
        public float customSpeed = 0.2f; 

        public override bool DoMovement()
        {
            if (!retroController.IsGrounded &&
                !doubleJumped && 
                retroController.TriedJumping > 0)
            {
                doubleJumped = true;

                if(allowSpeedThreshold || retroController.Velocity.y > threshold)
                {
                    // Jump
                    var walk = retroController.inputDir.y * transform.TransformDirection(Vector3.forward);
                    var strafe = retroController.inputDir.x * transform.TransformDirection(Vector3.right);
                    var wishDir = (walk + strafe).normalized;

                    var nextSpeed = Mathf.Max(retroController.Velocity.magnitude, retroController.Profile.JumpSpeed);
                    retroController.Velocity = wishDir * nextSpeed;
                    retroController.Velocity.y = customJumpSpeed ? customSpeed : retroController.Profile.JumpSpeed;
                }

                retroController.ResetJumping();

                retroController.CharacterMove(retroController.Velocity);
                return true;
            }
            return false;
        }
        public override void OnCharacterMove()
        {
            if (retroController.IsGrounded)
                doubleJumped = false;
        }
    }
}
