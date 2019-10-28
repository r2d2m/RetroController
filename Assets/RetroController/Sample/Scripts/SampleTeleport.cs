using UnityEngine;

namespace vnc.Samples
{
    public class SampleTeleport : MonoBehaviour {

        public RetroController retroController;

        public void Teleport()
        {
            retroController.TeleportTo(transform.position);
        }
    }
}
