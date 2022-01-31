using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using epiplon;

public class SwingTest : MonoBehaviour
{
    public RetroController controller;
    public Transform target;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            controller.TeleportTo(target.position);
        }
    }

}
