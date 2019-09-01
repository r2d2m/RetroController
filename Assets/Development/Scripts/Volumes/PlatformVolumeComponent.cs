using UnityEngine;

namespace vnc.Development
{
    public class PlatformVolumeComponent : TaggedVolumeComponent
    {
        [HideInInspector] public Vector3 startPoint, endPoint;
        [HideInInspector] public float speed = 6f;
        [HideInInspector] public float waitTime = 3f;

        [HideInInspector] public Rigidbody _rigidbody;
        [HideInInspector] public MeshFilter meshFilter;

        [HideInInspector] public Vector3 currentPoint;
        private float timer = 0;

        private RetroController player;

        protected void Start()
        {
            //meshFilter = gameObject.AddComponent<MeshFilter>();
            //meshFilter.sharedMesh = meshCollider.sharedMesh;

            //_rigidbody = gameObject.AddComponent<Rigidbody>();
            //_rigidbody.useGravity = false;
            //_rigidbody.isKinematic = true;
            //_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //_rigidbody.position = currentPoint = startPoint;

            player = FindObjectOfType<RetroController>();
        }

        private void FixedUpdate()
        {
            // don't move while wating
            if (Time.time < timer)
                return;

            var prevPosition = _rigidbody.position;
            Vector3 targetPosition = Vector3.MoveTowards(_rigidbody.position, currentPoint, speed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(targetPosition);
            Vector3 diff = targetPosition - prevPosition;

            if (_rigidbody.position.Equals(currentPoint))
            {
                if (currentPoint == startPoint) currentPoint = endPoint;
                else currentPoint = startPoint;

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
            //else if (retroLedgeGrab != null)
            //{
            //    if (retroLedgeGrab.OnLedge && retroLedgeGrab != null)
            //    {
            //        return retroLedgeGrab.GrabbingTarget.gameObject.Equals(gameObject);
            //    }
            //}

            return false;
        }

    }
}
