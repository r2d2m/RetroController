using UnityEngine;

namespace vnc.Samples
{
    public class SampleGuns : MonoBehaviour
    {
        public RetroController retroController;
        public GameObject doubleShotgun;
        public GameObject rocketLauncher;
        public GameObject explosion;
        public Transform m_camera;
        public LayerMask hitLayer;
        public LayerMask playerLayer;
        public float explosionForce = 0.5f;

        int weapon = 1;
        GameObject explosiveSphere = null;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) weapon = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) weapon = 2;

            doubleShotgun.SetActive(weapon == 1);
            rocketLauncher.SetActive(weapon == 2);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                switch (weapon)
                {
                    case 1: break;
                    case 2:
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
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
