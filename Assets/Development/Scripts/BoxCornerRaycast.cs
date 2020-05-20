using UnityEngine;
using vnc;

public class BoxCornerRaycast : MonoBehaviour
{

    public BoxCollider _boxCollider;
    public RetroControllerProfile Profile;
    public float gravity = 3f;
    const float EPSILON = 0.001f;
    RaycastHit[] stairGroundHit = new RaycastHit[4];

    private void FixedUpdate()
    {
        BoxEdgesRaycast(-transform.up);
    }

    public void BoxEdgesRaycast(Vector3 direction)
    {
        float distance = Profile.Gravity + _boxCollider.bounds.extents.y + EPSILON;
        DebugExtension.DebugArrow(transform.position, transform.forward, Color.blue);
        DebugExtension.DebugArrow(transform.position, transform.right, Color.red);

        Vector3 halfSize = _boxCollider.size / 2f;

        Vector3[] origins = new[]
        {
            transform.position + (transform.forward * halfSize.z) + (transform.right * halfSize.x),
            transform.position + (transform.forward * halfSize.z) + (-transform.right * halfSize.x),
            transform.position + (-transform.forward * halfSize.z) + (transform.right * halfSize.x),
            transform.position + (-transform.forward * halfSize.z) + (-transform.right * halfSize.x)
        };

        for (int i = 0; i < origins.Length; i++)
        {
            Debug.DrawLine(origins[i], origins[i] + (direction * distance), Color.yellow, Time.fixedDeltaTime);
        }

    }
}
