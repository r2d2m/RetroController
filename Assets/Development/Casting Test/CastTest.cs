using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vnc.Utils;

public class CastTest : MonoBehaviour
{

    public BoxCollider box;
    public CapsuleCollider capsule;
    public LayerMask contactLayer;

    RaycastHit boxhit, capsulehit;
    bool boxHitting, capsuleHitting;
    Vector3 origin;

    private void Awake()
    {
        origin = transform.position;
    }

    void Update()
    {
        boxHitting = PhysicsExtensions.BoxCast(box, Vector3.down, out boxhit, layerMask: contactLayer);
        capsuleHitting = PhysicsExtensions.CapsuleCast(capsule, Vector3.down, out capsulehit, layerMask: contactLayer);
            
        transform.position += Vector3.forward * Time.deltaTime;

        if (transform.position.z > 9)
            transform.position = origin;
    }

    private void OnDrawGizmos()
    {
        if (box)
        {
            DebugExtension.DrawBounds(box.bounds, Color.cyan);
        }

        if (capsule)
        {
            Vector3 start, end;
            start = capsule.transform.position + capsule.center + (Vector3.up * (capsule.height / 2f));
            end = capsule.transform.position + capsule.center + (Vector3.down * (capsule.height / 2f));
            float radius;
            PhysicsExtensions.ToWorldSpaceCapsule(capsule, out start, out end, out radius);
            DebugExtension.DrawCapsule(start, end, Color.yellow, radius);
        }

        if(boxHitting)
            DebugExtension.DrawPoint(boxhit.point, Color.cyan);

        if(capsuleHitting)
            DebugExtension.DrawPoint(capsulehit.point, Color.yellow);
    }
}
