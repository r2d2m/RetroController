using UnityEngine;
using vnc.Utils;

namespace vnc.Development
{
    public class SweepTest : MonoBehaviour
    {
        public Rigidbody _rigidbody;
        public Mesh mesh;
        public Vector3 direction;
        public float distance;
        public float step = 0.7f;

        Vector3 finalPosition;

        public float gizmoHitSize = 0.1f;

        private void FixedUpdate()
        {
            DebugExtension.DebugArrow(transform.position, direction * distance, Color.yellow);

            if (_rigidbody.SweepTest(direction, out RaycastHit hit, distance, QueryTriggerInteraction.UseGlobal))
            {
                DebugExtension.DrawBox(hit.point, Vector3.one * 0.1f, Quaternion.identity, Color.red);
                DebugExtension.DebugArrow(hit.point, hit.normal, Color.magenta, duration: Time.deltaTime);

                var vec = hit.point - _rigidbody.position;
                var projection = Vector3.Project(vec, direction);
                // projection
                DebugExtension.DebugArrow(transform.position, projection, Color.white, duration: Time.deltaTime);
                var closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
                closestPoint -= _rigidbody.position;
                finalPosition = _rigidbody.position + projection - closestPoint;

                CalculateOnStepPosition();
            }
            else
            {
                finalPosition = _rigidbody.position + (direction * distance);
            }

        }

        void CalculateOnStepPosition()
        {
            var originalPosition = _rigidbody.position;

            // step up
            _rigidbody.position += transform.up * step;
            if (_rigidbody.SweepTest(direction, out RaycastHit hit, distance))
            {
                var vec = hit.point - _rigidbody.position;
                var projection = Vector3.Project(vec, direction);
                var closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
                closestPoint -= _rigidbody.position;
                finalPosition = _rigidbody.position + projection - closestPoint;
            }
            else
            {
                finalPosition = _rigidbody.position + (direction * distance);
            }

            // step down
            _rigidbody.position = finalPosition;
            var down = -transform.up;
            if (_rigidbody.SweepTest(down, out hit, step))
            {
                var vec = hit.point - _rigidbody.position;
                var projection = Vector3.Project(vec, down);
                var closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
                closestPoint -= _rigidbody.position;
                finalPosition = _rigidbody.position + projection - closestPoint;
            }
            else
            {
                finalPosition = _rigidbody.position + (down * step);
            }


            // return rigidbody to original position because we are testing
            _rigidbody.position = originalPosition;
        }

        private void OnDrawGizmos()
        {
            // project location after solved collision
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(mesh, finalPosition);
        }
    }
}
