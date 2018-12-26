using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vnc.Utils;

namespace vnc.Editor.Utils
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
