using epiplon.Utils;
using UnityEditor;
using UnityEngine;

namespace epiplon.Editor.Utils
{
    [CustomPropertyDrawer(typeof(EditDisabledAttribute))]
    public class EditDisabledPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}