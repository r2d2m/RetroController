using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace vnc.Samples
{
    public class SampleUI : MonoBehaviour
    {
        public RetroController player;
        public TargetOption[] targetOptions;

        public static SampleUI Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void OnTeleport(int option)
        {
            Debug.Log("Option " + option + " chosen");
            if (option < targetOptions.Length)
            {
                player.TeleportTo(targetOptions[option].targetPoint.position);
            }
        }

        [System.Serializable]
        public struct TargetOption
        {
            public Transform targetPoint;
        }
    }
}