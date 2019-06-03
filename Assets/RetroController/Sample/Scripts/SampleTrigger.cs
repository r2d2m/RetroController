using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vnc.Samples
{
    [RequireComponent(typeof(BoxCollider))]
    public class SampleTrigger : MonoBehaviour
    {
        BoxCollider _box;
        public LayerMask playerLayer;

        public void FixedUpdate()
        {
            if (Physics.CheckBox(_box.center + transform.position, _box.size, transform.rotation, playerLayer, QueryTriggerInteraction.Ignore))
            {
                //TODO: set message in the player screen
            }
        }

    }

}
