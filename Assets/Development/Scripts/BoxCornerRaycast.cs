using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCornerRaycast : MonoBehaviour {

    public BoxCollider boxCollider;
    public float gravity = 3f;
    RaycastHit[] stairGroundHit = new RaycastHit[4];

    public virtual bool BoxEdgesRaycast(out int n)
    {
        Vector3[] origins = new[]
        {
            transform.position + (Vector3.forward * boxCollider.bounds.extents.z) + (Vector3.right * boxCollider.bounds.extents.x),
            transform.position + (Vector3.forward * boxCollider.bounds.extents.z) + (Vector3.left * boxCollider.bounds.extents.x),
            transform.position + (Vector3.back * boxCollider.bounds.extents.z) + (Vector3.right * boxCollider.bounds.extents.x),
            transform.position + (Vector3.back * boxCollider.bounds.extents.z) + (Vector3.left * boxCollider.bounds.extents.x)
        };
        for (int i = 0; i < origins.Length; i++)
        {
            n = Physics.RaycastNonAlloc(origins[i], Vector3.down, stairGroundHit, gravity, 0, QueryTriggerInteraction.Ignore);
            if (n > 0)
                return true;
        }

        n = 0;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3[] origins = new []
        {
            transform.position + (Vector3.forward * boxCollider.bounds.extents.z) + (Vector3.right * boxCollider.bounds.extents.x),
            transform.position + (Vector3.forward * boxCollider.bounds.extents.z) + (Vector3.left * boxCollider.bounds.extents.x),
            transform.position + (Vector3.back * boxCollider.bounds.extents.z) + (Vector3.right * boxCollider.bounds.extents.x),
            transform.position + (Vector3.back * boxCollider.bounds.extents.z) + (Vector3.left * boxCollider.bounds.extents.x)
        };
        for (int i = 0; i < origins.Length; i++)
        {
            Gizmos.DrawRay(origins[i], Vector3.down * gravity);
        }
    }
}
