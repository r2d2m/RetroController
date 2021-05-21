using UnityEngine;
using vnc.Utils;

namespace vnc.Development
{
    public class SweepTest : MonoBehaviour
    {
        public Rigidbody _rigidbody;
        public CapsuleCollider _collider;
        public Mesh mesh;
        public Vector3 direction;
        public float distance;
        public float step = 0.7f;

        RaycastHit hit = new RaycastHit();
        Vector3 projection;
        Vector3 closestPoint;
        Vector3 finalPosition;

        public float gizmoHitSize = 0.1f;

        private void FixedUpdate()
        {
            _rigidbody.SweepTest(direction, out hit, distance, QueryTriggerInteraction.UseGlobal);
            var vec = hit.point - _rigidbody.position;
            projection = Vector3.Project(vec, direction);
            closestPoint = _rigidbody.ClosestPointOnBounds(_rigidbody.position + projection);
            closestPoint -= _rigidbody.position;
            finalPosition = _rigidbody.position + projection - closestPoint;

            CalculateOnStepPosition();
        }

        void CalculateOnStepPosition()
        {
            // cast up
            finalPosition = transform.position + projection;
            bool foundUp = CastTo(finalPosition, transform.up, step, out RaycastHit hitInfo);

            // cast down
            finalPosition += transform.up * step;
            if (CastTo(finalPosition, -transform.up, step, out hitInfo))
                finalPosition += (-transform.up) * hitInfo.distance;
        }

        bool CastTo(Vector3 position, Vector3 castDirection, float castDistance, out RaycastHit hitInfo, bool draw = false)
        {
            _collider.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
            var dir = position - _rigidbody.position;
            point0 += dir;
            point1 += dir;
            
            if(draw)
                DebugExtension.DrawCapsule(point0, point1, Color.cyan, _collider.radius);
    
            return Physics.CapsuleCast(point0, point1, radius, castDirection, out hitInfo, castDistance);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.point, gizmoHitSize);


            // full movement
            DebugExtension.DebugArrow(transform.position, direction * distance, Color.yellow, duration: Time.deltaTime);

            // projection
            DebugExtension.DebugArrow(transform.position, projection, Color.white, duration: Time.deltaTime);

            // project location after solved collision
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(mesh, finalPosition);

            // hit normal
            DebugExtension.DebugArrow(hit.point, hit.normal, Color.magenta, duration: Time.deltaTime);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireMesh(mesh, transform.position + projection);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireMesh(mesh, finalPosition);
        }

        Rect r = new Rect(0, 0, 300, 300);
        private void OnGUI()
        {
            var d = Vector3.Dot(hit.normal, Vector3.up);
            GUI.Label(r, $"Normal: {hit.normal}\nDot: {d}");
        }
    }
}
