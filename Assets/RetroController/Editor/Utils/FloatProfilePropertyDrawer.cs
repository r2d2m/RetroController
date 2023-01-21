using epiplon.Utils;
using UnityEditor;
using UnityEngine;

namespace epipon.Editor
{
    [CustomPropertyDrawer(typeof(FloatProfileProperty))]
    public class FloatProfilePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var floatProperty = property.FindPropertyRelative("Value");
            floatProperty.floatValue = EditorGUI.FloatField(position, label, floatProperty.floatValue);
        }
    }
}
