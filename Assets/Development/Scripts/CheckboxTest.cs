using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxTest : MonoBehaviour {

    public BoxCollider boxCollider;
    public LayerMask layer;

    public bool isColliding;

    private void FixedUpdate()
    {
        var center = transform.position + boxCollider.center;
        isColliding = Physics.CheckBox(center, boxCollider.bounds.extents, boxCollider.transform.rotation, layer);
    }
}
