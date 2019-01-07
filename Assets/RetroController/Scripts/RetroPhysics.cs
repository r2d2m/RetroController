using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vnc
{
    /// <summary>
    /// Physics functions for the controller
    /// </summary>
    public static class RetroPhysics
    {
        public const float OVERBOUNCE = 1.01f;

        /// <summary>
        /// Clip the Velocity against the planes
        /// </summary>
        /// <param name="vel">The Velocity being clipped</param>
        /// <param name="normal">The plane normal</param>
        /// <param name="overbounce">Bounce back a little</param>
        /// <returns>The resulting Velocity</returns>
        public static Vector3 ClipVelocity(Vector3 velocity, Vector3 normal, bool overbounce)
        {
            var d = Vector3.Dot(velocity, normal);

            if (overbounce)
            {
                // q3 overbounce
                if (d < 0)
                {
                    d *= OVERBOUNCE;
                }
                else
                {
                    d /= OVERBOUNCE;
                }
            }

            velocity -= d * normal;
            return velocity;
        }
    }

}
