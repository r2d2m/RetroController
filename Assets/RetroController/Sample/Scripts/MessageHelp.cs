using UnityEngine;
namespace vnc.Samples
{
    public class MessageHelp : MonoBehaviour {

        [TextArea]
        public string m_message;

        [Space]
        public Transform rotatedMesh;
        public float rotationSpeed = 6;
        public float delay = 10;
        float timer = 0;

        private void Update()
        {
            if(rotatedMesh)
                rotatedMesh.localEulerAngles += Vector3.up * rotationSpeed * Time.deltaTime;
        }

        public void WriteMessage()
        {
            if(Time.time > timer)
            {
                SampleUI.Instance.Write(m_message);
                timer = Time.time + delay;
            }
        }
    }
}

