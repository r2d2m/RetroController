using UnityEditor;
using UnityEngine;
using vnc.Editor.Utils;

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
            EditorGUILayout.Space();

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

            DrawDefaultInspectorWithoutScriptField();
        }

        public bool DrawDefaultInspectorWithoutScriptField()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            SerializedProperty Iterator = serializedObject.GetIterator();

            Iterator.NextVisible(true);

            while (Iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(Iterator, true);
            }

            serializedObject.ApplyModifiedProperties();

            return (EditorGUI.EndChangeCheck());
        }
    }

}
