using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor
{
    [CustomEditor(typeof(RetroController))]
    public class RetroControllerEditor : UnityEditor.Editor
    {
        SerializedProperty profile;
        SerializedProperty view;

        private void OnEnable()
        {
            profile = serializedObject.FindProperty("Profile");
            view = serializedObject.FindProperty("controllerView");
        }

        public override void OnInspectorGUI()
        {
            if (profile.objectReferenceValue == null)
                EditorGUILayout.HelpBox("The controller doesn't have a Profile attached and won't work.", MessageType.Warning);
            else
            {
                if (GUILayout.Button("Open Profile"))
                {
                    //EditorGUIUtility.PingObject(profile.objectReferenceValue);
                    Selection.activeObject = profile.objectReferenceValue;
                }
            }

            if (view.objectReferenceValue == null)
                EditorGUILayout.HelpBox("The controller doesn't have a view object attached and won't work.", MessageType.Warning);

            EditorUtils.SetIcon(serializedObject.targetObject, "retro_controller");

            base.OnInspectorGUI();
        }
    }

}
