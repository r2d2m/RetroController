using UnityEngine;
using UnityEngine.Events;

namespace vnc.Samples
{
    /// <summary>
    /// The purpose of this is to show how can you propel the controller
    /// by simply adding a value to it's Velocity property.
    /// </summary>
    public class Rocket : MonoBehaviour
    {
        public float explosionForce = 0.5f;
        GameObject explosiveSphere = null;
        public GameObject explosion;
        public LayerMask hitLayer;
        public LayerMask playerLayer;

        RetroController retroController;
        Collider _collider;
        public DestroyCallback onDestroyCallback;

        public void OnCreate(Vector3 direction, RetroController retroController)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            _collider = GetComponent<Collider>();
            retroController.AddIgnoredCollider(_collider);
            Physics.IgnoreCollision(_collider, retroController.controllerCollider, true);

            this.retroController = retroController;
            onDestroyCallback = new DestroyCallback();
            onDestroyCallback.AddListener(() => retroController.RemoveIgnoredCollider(_collider));
        }

        Collider[] results = new Collider[4];
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == retroController.gameObject)
                return;

            if (explosiveSphere != null)
                Destroy(explosiveSphere);

            Vector3 medianPoint = Vector3.zero;
            for (int i = 0; i < collision.contacts.Length; i++)
                medianPoint += collision.contacts[i].point;

            medianPoint /= collision.contacts.Length;

            explosiveSphere = Instantiate(explosion, medianPoint, explosion.transform.rotation);
            Destroy(explosiveSphere, 2f);

            int n_col = Physics.OverlapSphereNonAlloc(medianPoint, 3, results, playerLayer);
            if (n_col > 0)
            {
                for (int i = 0; i < n_col; i++)
                {
                    var retroController = results[i].GetComponent<RetroController>();
                    if (retroController)
                    {
                        Vector3 dir = (retroController.transform.position - medianPoint).normalized;
                        retroController.Velocity += dir * explosionForce;
                    }
                }
            }

            Destroy(gameObject);
            onDestroyCallback.Invoke();
        }
    }

    [SerializeField]
    public class DestroyCallback : UnityEvent { };
}
