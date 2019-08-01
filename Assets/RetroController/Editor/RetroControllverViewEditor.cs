using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vnc;

namespace vnc.Editor
{
    [CustomEditor(typeof(RetroControllerView))]
    public class RetroControllverViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RetroControllerView retroControllerView = (RetroControllerView)target;

            //var rect = EditorGUILayout.GetControlRect();
            if(retroControllerView.playerView == null)
                EditorGUILayout.HelpBox("Player View field is empty, the View won't work.", MessageType.Error);

            if (retroControllerView.controllerCamera == null)
                EditorGUILayout.HelpBox("Controller Camera field is empty, the View won't work.", MessageType.Error);

            base.OnInspectorGUI();
        }
    }
}
