using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrafing : MonoBehaviour {

    Vector3 wishdir;
    float wishspeed;
    public float wishspeedScale = 4;

    Vector3 velocity;
    public float airAccelerate = 2;
    public float maxSpeed = 4;

    public float gravity = 0;
    public float velocityScale;
    public float rotate;

    private void Start()
    {
        InitStatus();
    }

    private void InitStatus()
    {
        velocity = transform.TransformDirection(Vector3.forward) * velocityScale;
        gravity = 0f;
        currentSpeed = 0;
    }

    void Update () {
        float fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        float side = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);

        rotate = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);

        transform.rotation *= Quaternion.Euler(0, rotate, 0);

        var walk = fwd * transform.TransformDirection(Vector3.forward);
        var strafe = side * transform.TransformDirection(Vector3.right);
        wishdir = (walk + strafe) * wishspeedScale;
        wishspeed = wishdir.magnitude;
        //wishdir.Normalize();
        
        Accelerate();
        velocity += Vector3.down * gravity;
    }

    float currentSpeed, addspeed;
    void Accelerate()
    {
        if(wishspeed > maxSpeed)
        {
            float scale = maxSpeed / wishspeed;
            wishdir *= scale;
            wishspeed = maxSpeed;
        }

        float wishspd = wishdir.magnitude;
        if (wishspd > 0)
            wishspd = 0;

        currentSpeed = Vector3.Dot(velocity, wishdir);
        addspeed = wishspd - currentSpeed;
        if (addspeed <= 0)
            return;

        var accelSpeed = airAccelerate * wishspeed * Time.deltaTime;
        if (accelSpeed > addspeed)
            accelSpeed = addspeed;

        velocity += accelSpeed * wishdir;

        // debug
        //velocity.Normalize();
    }

    public void OnGUI()
    {
        Rect rect = new Rect(0, 0, 300, 300);
        GUI.Label(rect, "Gravity: " + gravity);
        rect.y += 30;
        gravity = GUI.HorizontalSlider(new Rect(0, rect.y, 200, 30), gravity, -10, 10);
        rect.y += 30;
        if (GUI.Button(new Rect(0, rect.y, 200, 30), "Reset Velocity"))
        {
            InitStatus();
        }
        rect.y += 30;
        string status = "\n velocity: " + velocity
            + "\n wishdir: " + wishdir
            + "\n wishspeed: " + wishspeed
            + "\n addspeed: " + addspeed
            + "\n currentSpeed: " + currentSpeed;

        GUI.Label(rect, status);
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawArrow(transform.position, wishdir, Color.yellow);
        DebugExtension.DrawArrow(transform.position, velocity, Color.red);
    }
}
