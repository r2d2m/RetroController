using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vnc
{
    public class Platform : MonoBehaviour
    {
        // Basic
        BoxCollider m_Collider;
        Rigidbody m_rb;

        [Header("Movement Flow")]
        public Activation activationType;   // what makes the platform moves
        public bool Propell = false;
        public OnPathFinished onPathFinished;
        [Tooltip("The amount of distance the platform will travel each iteration")]
        public float Speed;                                 // Platform movement speed
        public bool isMoving { get; private set; }      // probably redundant with 'activated'

        // manage platform waiting in certain positions
        float timer = 0;
        float currentWaitTime = 0;

        [HideInInspector] public Vector3 Velocity { get; private set; }
        [HideInInspector] public PathNode[] paths;
        [HideInInspector] public int pathIndex = 0;
        [HideInInspector] public int pathFlowSign = 1;           // used to reverse the path
        [HideInInspector] public bool hitPlayer = false;      // revert the direction when hitting player
        private bool activated = false;     // platform only moves when activated

        private void Awake()
        {
            m_Collider = GetComponent<BoxCollider>();
            m_rb = GetComponent<Rigidbody>();
            m_rb.isKinematic = true;
            //gameObject.layer = LayerMask.NameToLayer("Platform");

            if (activationType == Activation.Nothing)
                activated = true;
        }

        private void FixedUpdate()
        {
            if (paths == null)
                return;
            else if (paths.Length == 0)
                return;

            if (!activated)
                return;

            if (Time.time < timer + currentWaitTime)
            {
                isMoving = false;
            }
            else
            {
                CommonMove();
                if (transform.position == paths[pathIndex].Position)
                {
                    currentWaitTime = paths[pathIndex].WaitTime;
                    timer = Time.time;

                    pathIndex += pathFlowSign;

                    if (pathIndex == paths.Length
                        || pathIndex < 0)
                    {
                        switch (onPathFinished)
                        {
                            case OnPathFinished.Restart:
                                pathIndex = 0;
                                m_rb.MovePosition(paths[0].Position);

                                // if requires Player, deactivate
                                if (activationType == Activation.Player)
                                    activated = false;

                                break;
                            case OnPathFinished.Reverse:
                                pathFlowSign *= -1;
                                pathIndex += pathFlowSign;

                                // if requires Player, deactivate
                                // on the start of the path
                                if (pathIndex == 0 && activationType == Activation.Player)
                                    activated = false;

                                break;
                            case OnPathFinished.Stop:
                                pathIndex -= 1;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        void CommonMove()
        {
            Vector3 nextPos = Vector3.MoveTowards(m_rb.position, paths[pathIndex].Position, Speed * Time.deltaTime);
            Velocity = nextPos - m_rb.position;
            m_rb.MovePosition(nextPos);
            isMoving = true;
            if (hitPlayer)
            {
                pathIndex = Mathf.Clamp(pathIndex - pathFlowSign, 0, paths.Length - 1);
                hitPlayer = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                if (activationType == Activation.Player)
                    activated = true;
            }
        }

        // what makes the platform activate
        public enum Activation
        {
            Player,
            Trigger,
            Nothing
        }

        public enum OnPathFinished
        {
            Restart,
            Reverse,
            Stop
        }

    }

    public struct PathNode
    {
        public int ID;
        public Vector3 Position;
        public float WaitTime;
    }
}
