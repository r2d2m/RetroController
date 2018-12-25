using UnityEditor;
using UnityEngine;
using vnc.Editor.Utils;

namespace vnc.Editor
{
    [CustomEditor(typeof(RetroControllerProfile))]
    public class RetroControllerProfileEditor : UnityEditor.Editor
    {
        SerializedProperty waterTag, ladderTag, platformTag;
        GUIStyle labelStyle = null;

        private void OnEnable()
        {
            waterTag = serializedObject.FindProperty("WaterTag");
            ladderTag = serializedObject.FindProperty("LadderTag");
            platformTag = serializedObject.FindProperty("PlatformTag");

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.helpBox);
                labelStyle.font = EditorStyles.boldFont;
                labelStyle.fontSize = 14;
                labelStyle.alignment = TextAnchor.MiddleLeft;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(serializedObject.targetObject.name, labelStyle);
            EditorGUILayout.Space();

            DrawDefaultInspectorWithoutScriptField();

            EditorUtils.SetIcon(serializedObject.targetObject, "retro_controller_profile");
            EditorGUILayout.Space();
            waterTag.stringValue = EditorGUILayout.TagField("Water Tag", waterTag.stringValue);
            ladderTag.stringValue = EditorGUILayout.TagField("Ladder Tag", ladderTag.stringValue);
            platformTag.stringValue = EditorGUILayout.TagField("Platform Tag", platformTag.stringValue);


            serializedObject.ApplyModifiedProperties();
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
