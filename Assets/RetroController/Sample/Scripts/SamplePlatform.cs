using UnityEngine;
using vnc.Movements;
//using vnc.Movements;

namespace vnc.Samples
{
    /// <summary>
    /// This sample script shows how can you implement a platform
    /// for your game, making it possible to move the controller
    /// when it's standing on the platform.
    /// </summary>
    public class SamplePlatform : MonoBehaviour
    {
        RetroController player;
        RetroLedgeGrab retroLedgeGrab;
        Rigidbody _rigidbody;
        Collider _collider;

        public Vector3[] points;
        public float speed = 6f;
        public float waitTime = 3f;
        public RigidbodyInterpolation rigidbodyInterpolation = RigidbodyInterpolation.Interpolate;
        int index = 0;
        float timer;

        public void Awake()
        {
            player = FindObjectOfType<RetroController>();

            if (points.Length > 0)
                transform.position = points[0];

            if (player == null)
                Debug.LogWarning("No Retro Controller Player assigned to platform " + name);
            else
                retroLedgeGrab = player.GetComponentInChildren<RetroLedgeGrab>();

            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
                _rigidbody = gameObject.AddComponent<Rigidbody>();

            _rigidbody.hideFlags = HideFlags.HideAndDontSave;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _collider = GetComponentInChildren<Collider>();
        }

        public void FixedUpdate()
        {
            _rigidbody.interpolation = rigidbodyInterpolation;

            // don't move while wating
            if (Time.time < timer)
                return;

            // move the platform
            var prevPosition = _rigidbody.position;
            Vector3 targetPosition = Vector3.MoveTowards(_rigidbody.position, points[index], speed);
            _rigidbody.MovePosition(targetPosition);
            Vector3 diff = targetPosition - prevPosition;

            // change index when reaching target
            if (_rigidbody.position.Equals(points[index]))
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

                return player.CurrentPlatform.Equals(_collider);
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

        private void OnDrawGizmosSelected()
        {
            if (points != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < points.Length; i++)
                {
                    Gizmos.DrawSphere(points[i], 1f);
                    if (i > 0)
                        Gizmos.DrawLine(points[i], points[i - 1]);
                }
            }
        }
    }

}
