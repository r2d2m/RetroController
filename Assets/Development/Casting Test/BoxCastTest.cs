using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{

    BoxCollider box;
    RaycastHit hit;
    public LayerMask contactLayer;
    public float movement;
    bool boxHitting;


    void Start()
    {
        box = GetComponent<BoxCollider>();
    }


    void FixedUpdate()
    {
        if (boxHitting = Physics.BoxCast(transform.position, box.size / 2, transform.forward, out hit, transform.rotation, movement,
            layerMask: contactLayer))
        {

        }
    }

    private void OnGUI()
    {
        float distance = movement;
        if (boxHitting)
        {
            distance = hit.distance;
        }

        GUI.Label(new Rect(0, 0, 100, 100), "Distance: " + distance);
    }

    private void OnDrawGizmos()
    {
        Vector3 e = Vector3.one * 0.01f;
        if (box)
        {
            // current position
            Gizmos.color = Color.white;
            Gizmos.DrawCube(transform.position, box.size);
        }

        if (boxHitting)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + (transform.forward * hit.distance), box.size);
        }

    }
}
