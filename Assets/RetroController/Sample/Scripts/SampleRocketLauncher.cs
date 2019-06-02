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
        public float shootingDelay = 0.3f;
        float time;

        GameObject explosiveSphere = null;

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && retroController.updateController)
            {
                if (Time.time < time + shootingDelay)
                    return;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, m_camera.forward, out hit, Mathf.Infinity, hitLayer, QueryTriggerInteraction.Ignore))
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

                    time = Time.time;
                }
            }
        }
    }
}
