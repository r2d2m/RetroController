using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace vnc.Development
{
    public class DefaultPlayer : MonoBehaviour
    {
        public CharacterController characterController;
        public Camera _camera;
        public MouseLook mouseLook;
        public float speed = 6f;

        private void Start()
        {
            mouseLook.Init(transform, _camera.transform);
        }

        void Update()
        {
            var fwd = transform.forward * Input.GetAxisRaw("Vertical");
            var strafe = transform.right * Input.GetAxisRaw("Horizontal");
            var wishdir = (fwd + strafe).normalized;

            characterController.Move(wishdir * speed);

            mouseLook.LookRotation(transform, _camera.transform);
            mouseLook.UpdateCursorLock();
        }
    }
}
