using System.Collections.Generic;
using UnityEngine;

namespace vnc
{
    public class RetroControllerSystem : MonoBehaviour
    {
        public static RetroControllerSystem Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            var retroControllerSystemObj = new GameObject("RETRO CONTROLLER SYSTEM");
            DontDestroyOnLoad(retroControllerSystemObj);
            Instance = retroControllerSystemObj.AddComponent<RetroControllerSystem>();
        }

        public List<RetroController> Controllers { get; private set; } = new List<RetroController>();

        private void FixedUpdate()
        {
            foreach (var controller in Controllers)
            {
                if(controller.updateController)
                    controller.UpdateController();
            }
        }
    }

}
