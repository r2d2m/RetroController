using UnityEngine;

namespace vnc.Samples
{
    public class SamplePlayer : MonoBehaviour
    {
        public RetroController retroController;

        void Update()
        {
            float fwd = (Input.GetButton("Forward") ? 1 : 0) - (Input.GetButton("Backwards") ? 1 : 0);
            float strafe = (Input.GetButton("Strafe_Right") ? 1 : 0) - (Input.GetButton("Strafe_Left") ? 1 : 0);
            float swim = Input.GetAxisRaw("Swim");
            bool jump = Input.GetButtonDown("Jump");
            bool sprint = Input.GetButton("Sprint");

            retroController.SetInput(fwd, strafe, swim, jump, sprint);
        }
    }
}
