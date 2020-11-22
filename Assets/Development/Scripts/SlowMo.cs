using UnityEngine;

namespace vnc.Development
{
    public class SlowMo : MonoBehaviour
    {
        public float value = 1f;

        void Update()
        {
            Time.timeScale = value;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
