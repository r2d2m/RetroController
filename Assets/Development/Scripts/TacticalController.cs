using UnityEngine;

namespace vnc.Development
{
    public class TacticalController : RetroController
    {
        public override Vector3 Accelerate(Vector3 wishdir, Vector3 prevVelocity, float accelerate, float max_velocity)
        {
            //return wishdir * accelerate;

            var projVel = Vector3.Dot(prevVelocity, wishdir);
            float accelSpeed = accelerate * Time.fixedDeltaTime;

            if (projVel + accelSpeed > max_velocity)
                accelSpeed = max_velocity - projVel;

            Vector3 newVel = prevVelocity + wishdir * accelSpeed;
            return newVel;
        }

        //public override Vector3 Friction(Vector3 prevVelocity, float friction)
        //{
        //    return prevVelocity;
        //}

        protected override void AccelerateAir(Vector3 wishDir, float wishSpeed, float accelerate, float maxSpeed)
        {
            //// TODO: strip gravity direction for all directions
            var gravity = Velocity.y;

            var tempDir = Velocity;
            tempDir.y = 0;

            var newVel = tempDir + (wishDir * accelerate * Profile.MaxAirControl);
            if (newVel.magnitude > maxSpeed)
                newVel = newVel.normalized * maxSpeed;

            Velocity = newVel + (Vector3.up * gravity);
        }
    }
}