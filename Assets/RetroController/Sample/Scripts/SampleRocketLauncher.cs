﻿using UnityEngine;

namespace vnc.Samples
{
    /// <summary>
    /// This is a sample script for a rocket launcher type of weapon.
    /// </summary>
    public class SampleRocketLauncher : MonoBehaviour
    {
        public RetroController retroController;

        [Space]
        public Rigidbody _rocket;
        public float _rocketSpeed = 6;
        public Transform _rocketSpawn;
        public float shootingDelay = 0.3f;
        float time;
        bool shoot;

        [Space]
        public Transform m_camera;
        public Animator _animator;
        Collider _collider;

        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        void Update()
        {
            if (Input.GetMouseButton(0) && retroController.updateController)
            {
                if (Time.time < time + shootingDelay)
                    return;

                shoot = true;

                _animator.SetTrigger("Fire");
                time = Time.time;
            }
        }

        void FixedUpdate()
        {
            if (shoot)
            {
                var rocketInstance = Instantiate(_rocket, _rocketSpawn.position, _rocket.transform.rotation);
                rocketInstance.transform.rotation = Quaternion.LookRotation(m_camera.forward);
                rocketInstance.AddForce(rocketInstance.transform.forward * _rocketSpeed, ForceMode.Force);

                Physics.IgnoreCollision(rocketInstance.GetComponent<Collider>(), _collider, true);

                shoot = false;
            }
        }
    }
}
