using UnityEngine;

namespace vnc.Development
{
    public class CommonController : RetroController
    {
        [Header("Debug")]
        public float velDot;

        //public override Vector3 Accelerate(Vector3 wishdir, Vector3 prevVelocity, float accelerate, float max_velocity)
        //{
        //    return wishdir * accelerate;
        //}

        public override Vector3 Friction(Vector3 prevVelocity, float friction)
        {
            var wishspeed = prevVelocity;

            float speed = wishspeed.magnitude;

            if (speed != 0) // To avoid divide by zero errors
            {
                //float control = speed < Profile.MinimumSpeed ? Profile.MinimumSpeed : speed;
                float drop = speed * friction * Time.fixedDeltaTime;

                wishspeed *= Mathf.Max(speed - drop, 0) / speed; // Scale the Velocity based on friction.
            }
            return wishspeed;
        }

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

            ////Velocity = newDir * XZVel.magnitude + Vector3.down * gravity;
            //Velocity = Vector3.down * gravity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, wishDir * 10);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Velocity * 50);

            Gizmos.color = Color.green;
            var cross = Vector3.Cross(wishDir, Velocity);
            Gizmos.DrawRay(transform.position, cross * 50);
        }
    }
}