using UnityEngine;

public class StairCastTest : MonoBehaviour
{
    public BoxCollider box;
    public LayerMask contactLayer;
    public float stepOffset = 0.7f;
    public float movement = 2f;

    RaycastHit boxhit;
    bool boxHitting = false;

    Vector3 upPos, forwardPos, downPos, finalPos;


    void FixedUpdate()
    {
        upPos = transform.position + (Vector3.up * stepOffset);
        forwardPos = upPos + (Vector3.forward * movement);
        downPos = forwardPos + (Vector3.down * stepOffset);

        boxHitting = Physics.BoxCast(forwardPos, box.size / 2, Vector3.down, out boxhit, transform.rotation,
            stepOffset, layerMask: contactLayer);

        if (boxHitting)
        {
            finalPos = forwardPos + (Vector3.down * boxhit.distance);
            transform.position = finalPos;
        }
        else
        {
            transform.position += Vector3.forward * movement;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 e = Vector3.one * 0.01f;
        if (box)
        {
            // current position
            Gizmos.color = Color.white;
            Gizmos.DrawCube(transform.position, box.size);

            // up
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(upPos, box.size + e);

            // forward
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(forwardPos, box.size + e);

            if (boxHitting)
            {
                // final pos
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(finalPos, box.size + e);
            }

            //DebugExtension.DrawArrow(boxhit.point, boxhit.normal, Color.red);
        }

    }
}
