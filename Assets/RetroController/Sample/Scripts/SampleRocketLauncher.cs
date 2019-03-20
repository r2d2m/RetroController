using UnityEngine;

namespace vnc.Samples
{
    public class SampleRocketLauncher : MonoBehaviour
    {
        public RetroController retroController;
        public GameObject rocketLauncher;
        public GameObject explosion;
        public Transform m_camera;
        public LayerMask hitLayer;
        public LayerMask playerLayer;
        public float explosionForce = 0.5f;

        GameObject explosiveSphere = null;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, m_camera.forward, out hit, Mathf.Infinity, hitLayer))
                {
                    if (explosiveSphere != null)
                        Destroy(explosiveSphere);

                    explosiveSphere = Instantiate(explosion, hit.point, explosion.transform.rotation);
                    Destroy(explosiveSphere, 2f);

                    if (Physics.CheckSphere(hit.point, 3, playerLayer))
                    {
                        Vector3 dir = (transform.position - hit.point).normalized;
                        retroController.Velocity += dir * explosionForce;
                    }
                }
            }
        }
    }
}
