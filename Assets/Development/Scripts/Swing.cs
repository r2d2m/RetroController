using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [Range(0, 180)]
    public int amplitude = 120;
    public float length = 5f;

    private void OnDrawGizmos()
    {
        int gap = (180 - amplitude) / 2;

        Gizmos.color = Color.green;
        for (int i = gap ; i < 180 - gap; i++)
        {
            var angle = Mathf.Deg2Rad * i;
            var x = Mathf.Cos(angle) * length;
            var y = Mathf.Sin(angle) * length;
            var point = transform.position - new Vector3(x, y);
            Gizmos.DrawSphere(point, .1f);
        }

        //Gizmos.DrawCube(startPoint, Vector3.one);
        //Gizmos.DrawCube(endPoint, Vector3.one);
        //Gizmos.DrawCube(middlePoint, Vector3.one);
    }
}
