using UnityEngine;

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

        Collider[] results = new Collider[4];
        private void OnCollisionEnter(Collision collision)
        {
            RaycastHit hit;
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
        }
    }
}
