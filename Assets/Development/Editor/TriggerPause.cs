using UnityEditor;
using UnityEngine;

namespace vnc.Development
{
    public class TriggerPause : MonoBehaviour
    {
        public void Pause()
        {
            EditorApplication.isPaused = true;
        }
    }
}
