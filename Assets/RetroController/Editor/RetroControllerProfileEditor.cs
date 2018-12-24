using UnityEditor;
using vnc.Editor.Utils;

namespace vnc.Editor
{
    [CustomEditor(typeof(RetroControllerProfile))]
    public class RetroControllerProfileEditor : UnityEditor.Editor
    {
        SerializedProperty waterTag, ladderTag, platformTag;

        private void OnEnable()
        {
            waterTag = serializedObject.FindProperty("WaterTag");
            ladderTag = serializedObject.FindProperty("LadderTag");
            platformTag = serializedObject.FindProperty("PlatformTag");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUtils.SetIcon(serializedObject.targetObject, "retro_controller_profile");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collision Tags", EditorStyles.boldLabel);
            waterTag.stringValue = EditorGUILayout.TagField("Water Tag", waterTag.stringValue);
            ladderTag.stringValue = EditorGUILayout.TagField("Ladder Tag", ladderTag.stringValue);
            platformTag.stringValue = EditorGUILayout.TagField("Platform Tag", platformTag.stringValue);


            serializedObject.ApplyModifiedProperties();
        }

    }
}
