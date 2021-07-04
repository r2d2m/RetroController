using System.Runtime.CompilerServices;
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
            var originalPosition = _rigidbody.position;
            DebugExtension.DebugArrow(originalPosition, direction * distance, Color.yellow);

            if (_rigidbody.SweepTest(direction, out RaycastHit hit, distance, QueryTriggerInteraction.UseGlobal))
            {
                DebugExtension.DrawBox(hit.point, Vector3.one * 0.1f, Quaternion.identity, Color.red);
                DebugExtension.DebugArrow(hit.point, hit.normal, Color.magenta, duration: Time.deltaTime);

                var vec = hit.point - _rigidbody.position;
                var projection = Vector3.Project(vec, direction);
                // projection
                DebugExtension.DebugArrow(originalPosition, projection, Color.white, duration: Time.deltaTime);
                var closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
                closestPoint -= _rigidbody.position;
                finalPosition = _rigidbody.position + projection - closestPoint;

                CalculateOnStepPosition();
            }
            else
            {
                finalPosition = _rigidbody.position + (direction * distance);
            }

            // return rigidbody to original position because we are testing
            _rigidbody.position = originalPosition;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CalculateOnStepPosition()
        {
            bool hitUp = Sweep(transform.up, step);
            if(!hitUp)
                _rigidbody.position += transform.up * step;

            bool hitDirection = Sweep(direction, distance);
            if(!hitDirection)
                _rigidbody.position += (direction * distance);

            bool hitDown = Sweep(-transform.up, step);
            if(!hitDown)
                _rigidbody.position += (-transform.up * step);

            finalPosition = _rigidbody.position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Sweep(Vector3 dir, float dist)
        {
            if (_rigidbody.SweepTest(dir, out RaycastHit hit, dist))
            {
                var vec = hit.point - _rigidbody.position;
                var projection = Vector3.Project(vec, dir);
                var closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
                closestPoint -= _rigidbody.position;
                _rigidbody.position += projection - closestPoint;
                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            // project location after solved collision
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(mesh, finalPosition);
        }
    }
}
