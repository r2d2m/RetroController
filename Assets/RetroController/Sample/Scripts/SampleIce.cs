using System.Reflection;
using UnityEngine;

namespace vnc.Samples
{
    public class SampleIce : MonoBehaviour
    {
        RetroController retroController;

        public float iceFriction = 1f;

        void Start()
        {
            retroController = FindObjectOfType<RetroController>();
        }

        public void OnEnterIce()
        {
            if (retroController)
            {
                //retroController.RuntimeScriptable.GetInt(2);
                //retroController.RuntimeScriptable.Set("GroundFriction", iceFriction);
            }
        }

        public void OnExitIce()
        {
            if (retroController)
            {
                //retroController.RuntimeScriptable.Reset(() => retroController.Profile.GroundFriction);
            }
        }
    }
}
