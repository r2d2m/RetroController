using UnityEngine;
namespace vnc.Samples
{
    public class MessageHelp : MonoBehaviour {

        [TextArea]
        public string m_message;

        [Space]
        public Transform rotatedMesh;
        public float rotationSpeed = 6;

        private void Update()
        {
            if(rotatedMesh)
                rotatedMesh.localEulerAngles += Vector3.up * rotationSpeed * Time.deltaTime;
        }

        public void WriteMessage()
        {
            SampleUI.Instance.Write(m_message);
        }
    }
}

