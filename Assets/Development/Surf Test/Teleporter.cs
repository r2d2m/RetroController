using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vnc;

public class Teleporter : MonoBehaviour {

    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var retrocontroller = other.GetComponent<RetroController>();
            retrocontroller.TeleportTo(destination.position);
            retrocontroller.Velocity = Vector3.zero;
        }
        
    }
}
