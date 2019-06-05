using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace vnc.Samples
{
    public class SampleUI : MonoBehaviour
    {
        public Transform player;
        public Text targetText;
        public string[] message;

        public static SampleUI Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void OnTeleport(int option)
        {
            //if (option >= eventOptions.Length)
            //{
            //    Debug.LogWarning("Invalid event option");
            //    return;
            //}

            //eventOptions[option].Invoke();
        }
    }
}