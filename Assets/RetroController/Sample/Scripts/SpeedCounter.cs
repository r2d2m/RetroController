using System;
using UnityEngine;
using UnityEngine.UI;

namespace vnc.Samples
{
    public class SpeedCounter : MonoBehaviour
    {
        public RetroController retroController;
        Vector3 lastPosition;
        public Text speedText;

        private void Awake()
        {
            retroController = FindObjectOfType<RetroController>();
        }

        void FixedUpdate()
        {
            if (retroController)
            {
                lastPosition.y = 0;
                var p = retroController.FixedPosition;
                p.y = 0;
                double distance = Vector3.Distance(lastPosition, p);
                distance = Math.Truncate((distance * 100));
                speedText.text = (distance / Time.fixedDeltaTime).ToString("0");
                lastPosition = retroController.FixedPosition;
            }
        }
    }

}
