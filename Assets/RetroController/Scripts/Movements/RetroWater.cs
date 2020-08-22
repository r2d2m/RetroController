using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vnc.Movements
{
    // TODO: currently using states inside the controller
    public class RetroWater : RetroMovement
    {
        public override bool DoMovement()
        {
#pragma warning disable 612, 618
            if (retroController.IsSwimming && retroController.WaterState == RetroController.CC_Water.Underwater)
            {
                var inputDir = retroController.inputDir;
                var controllerView = retroController.controllerView;
                var gravityDirection = retroController.gravityDirection;

                // player moved the character
                var walk = inputDir.y * controllerView.forward;
                var strafe = inputDir.x * transform.TransformDirection(Vector3.right);
                Vector3 wishDir = (walk + strafe) + (gravityDirection * retroController.Swim);
                wishDir.Normalize();

                retroController.ResetJumping();

                retroController.Velocity = MoveWater(wishDir, retroController.Velocity);
                retroController.AddGravity(RetroProfile.WaterGravityScale);

                retroController.CharacterMove(retroController.Velocity);
                retroController.OnDuckState();
                return true;
            }
#pragma warning restore 612, 618

            return false;
        }

        // move with water friction
        protected virtual Vector3 MoveWater(Vector3 accelDir, Vector3 prevVelocity)
        {
            prevVelocity = WaterFriction(prevVelocity);
            return retroController.Accelerate(accelDir, prevVelocity, RetroProfile.WaterAcceleration, RetroProfile.MaxWaterSpeed);
        }

        protected virtual Vector3 WaterFriction(Vector3 prevVelocity)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                float drop = speed * RetroProfile.WaterFriction * Time.fixedDeltaTime;
                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the Velocity based on friction.
            }
            return wishspeed;
        }

        public override void OnCharacterMove()
        {

        }
    }
}
