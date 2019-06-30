using UnityEngine;
using vnc.Movements;

namespace vnc.Samples
{
    /// <summary>
    /// This sample script shows how can you implement a platform
    /// for your game, making it possible to move the controller
    /// when it's standing on the platform.
    /// </summary>
    public class SamplePlatform : MonoBehaviour
    {
        public RetroController player;
        RetroLedgeGrab retroLedgeGrab;

        public Vector3[] points;
        public float speed = 6f;
        public float waitTime = 3f;
        int index = 0;
        float timer;

        public virtual void Start()
        {
            if (points.Length > 0)
                transform.position = points[0];

            if (player == null)
                Debug.LogWarning("No Retro Controller Player assigned to platform " + name);

            if (player != null)
                retroLedgeGrab = player.GetCustomMovement<RetroLedgeGrab>();
        }

        public virtual void FixedUpdate()
        {
            // don't move while wating
            if (Time.time < timer)
                return;

            // move the platform
            var prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, points[index], speed * Time.fixedDeltaTime);
            Vector3 diff = transform.position - prevPosition;

            // change index when reaching target
            if (transform.position.Equals(points[index]))
            {
                index++;
                if (index == points.Length)
                    index = 0;

                // wait
                timer = Time.time + waitTime;
            }

            if (player == null)
                return;

            if (OnPlatform())
            {
                player.CharacterMove(diff, runCustomMovements: false);
            }
        }

        /// <summary>
        /// Check for OnPlatform state on the controller
        /// </summary>
        /// <returns>True if is on this platform</returns>
        public virtual bool OnPlatform()
        {
            if (player.HasState(RetroController.CC_State.OnPlatform))
            {
                if (player.CurrentPlatform == null)
                    return false;

                return player.CurrentPlatform.gameObject.Equals(gameObject);
            }
            else if (retroLedgeGrab != null)
            {
                if (retroLedgeGrab.OnLedge && retroLedgeGrab != null)
                {
                    return retroLedgeGrab.GrabbingTarget.gameObject.Equals(gameObject);
                }
            }

            return false;
        }
    }

}
