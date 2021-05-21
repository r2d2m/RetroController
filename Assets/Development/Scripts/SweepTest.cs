using UnityEngine;

public class SweepTest : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public Mesh mesh;
    public Vector3 direction;
    public float distance;

    RaycastHit hit = new RaycastHit();
    Vector3 projection;

    public float gizmoHitSize = 0.1f;

    private void FixedUpdate()
    {
        _rigidbody.SweepTest(direction, out hit, distance);
        var vec = hit.point - transform.position;
        projection = Vector3.Project(vec, direction);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, gizmoHitSize);


        // full movement
        DebugExtension.DebugArrow(transform.position, direction * distance, Color.yellow, duration: Time.deltaTime);

        // projection
        DebugExtension.DebugArrow(transform.position, projection, Color.white, duration: Time.deltaTime);

        // closest point?
        var closestPoint = _rigidbody.ClosestPointOnBounds(transform.position + projection);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(mesh, new Vector3(1, 2, 1));
        var c = closestPoint - transform.position;

        // project location after solved collision
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(mesh, transform.position + projection - c);


        // hit normal
        DebugExtension.DebugArrow(hit.point, hit.normal, Color.magenta, duration: Time.deltaTime);
    }
}
