using UnityEngine;
using UnityEngine.UI;

namespace vnc.Development
{
    public class SpeedCounter : MonoBehaviour
    {
        public RetroController retroController;
        Vector3 lastPosition;
        public Text speedText;

        void FixedUpdate()
        {
            var distance = Vector3.Distance(lastPosition, retroController.FixedPosition);
            speedText.text = (distance / Time.fixedDeltaTime).ToString("0.0");
            lastPosition = retroController.FixedPosition;
        }
    }

}
