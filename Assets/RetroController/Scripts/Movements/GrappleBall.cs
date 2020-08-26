using UnityEngine;
using UnityEngine.Events;

namespace vnc.Movements
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrappleBall : MonoBehaviour {

        public float speed = 2;
        public float maxDistance = 60f;
        [HideInInspector] public HookEvent onHooked;

        Rigidbody _rigidbody;
        bool launched;
        Vector3 direction;
        float traveledDistance;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            onHooked = new HookEvent();
        }

        public void Launch(Vector3 origin, Vector3 dir)
        {
            direction = dir;
            launched = true;
            traveledDistance = 0;
            _rigidbody.MovePosition(origin);
        }

        private void FixedUpdate()
        {
            if (launched)
            {
                var distance = speed * Time.fixedDeltaTime;
                var velocity = direction * distance;
                _rigidbody.MovePosition(_rigidbody.position + velocity);

                traveledDistance += distance;
                if (traveledDistance >= maxDistance)
                    launched = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // object is on layer Default
            if (((1 << collision.gameObject.layer) & 1 << 0) != 0)
            {
                launched = false;
                onHooked.Invoke(collision.contacts[0].point);
                onHooked.RemoveAllListeners();
            }
        }
    }
}
