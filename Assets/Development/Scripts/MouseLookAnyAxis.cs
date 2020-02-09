using UnityEngine;
using vnc.Utils;

namespace vnc.Samples
{
    public class MouseLookAnyAxis : MouseLook
    {
        private RetroController characterController;
        private Transform characterCamera;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private float kick = 0;

        private float yRot = 0;
        private float xRot = 0;

        public override void Init(RetroController character, Transform camera)
        {
            characterController = character;
            characterCamera = camera;

            m_CharacterTargetRot = characterController.transform.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public override void LookRotation()
        {
            if (!lockCursor)
                return;

            kick -= (Time.deltaTime * cameraKickSpeed);
            kick = Mathf.Clamp(kick, 0, cameraKickOffset);

            yRot += Input.GetAxis("Mouse X") * mouseSensitivity;
            xRot += Input.GetAxis("Mouse Y") * mouseSensitivity;
            xRot = Mathf.Clamp(xRot, MinimumX, MaximumX);

            m_CharacterTargetRot = characterController.GetOnAxisRotation() * Quaternion.Euler(0, yRot, 0);
            m_CameraTargetRot = Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                characterController.transform.rotation = Quaternion.Slerp(characterController.transform.rotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                characterCamera.localRotation = Quaternion.Slerp(characterCamera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                characterController.transform.rotation = m_CharacterTargetRot;
                characterCamera.localRotation = m_CameraTargetRot;

                if (cameraKick)
                {
                    var x = Mathf.Clamp(kick, 0, cameraKickOffset - cameraKickoffsetWindow);
                    characterCamera.rotation *= Quaternion.Euler(-x, 0, 0);
                }
            }
        }


        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }


    }
}

