using UnityEngine;

namespace vnc.Samples
{
    /// <summary>
    /// This is a sample script for a rocket launcher type of weapon.
    /// The purpose of this is to show how can you propel the controller
    /// by simply adding a value to it's Velocity property.
    /// </summary>
    public class SampleRocketLauncher : MonoBehaviour
    {
        public RetroController retroController;

        [Space]
        public Rigidbody _rocket;
        public float _rocketSpeed = 6;
        public Transform _rocketSpawn;

        public Transform m_camera;
        
        public float shootingDelay = 0.3f;
        float time;        

        void Update()
        {
            if (Input.GetMouseButton(0) && retroController.updateController)
            {
                if (Time.time < time + shootingDelay)
                    return;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, m_camera.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    var rocketInstance = Instantiate(_rocket, _rocketSpawn.position, _rocket.transform.rotation);
                    rocketInstance.transform.LookAt(hit.point);
                    rocketInstance.AddForce(rocketInstance.transform.forward * _rocketSpeed, ForceMode.Force);

                    

                    time = Time.time;
                }
            }
        }
    }
}
