using UnityEngine;
using UnityEngine.Events;

namespace vnc.Samples
{
    [ExecuteInEditMode]
    public class SampleTrigger : MonoBehaviour
    {
        public LayerMask playerLayer;
        public UnityEvent unityEvent;

        private void OnTriggerEnter(Collider other)
        {
            // player is on selected layer mask
            if (Contains(playerLayer, other.gameObject.layer))
            {
                unityEvent.Invoke();
            }
        }

        bool Contains(LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}
