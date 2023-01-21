using epiplon.Utils;
using UnityEditor;
using UnityEngine;

namespace epipon.Editor
{
    [CustomPropertyDrawer(typeof(IntProfileProperty))]
    public class IntProfilePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var intProperty = property.FindPropertyRelative("Value");
            var value = EditorGUI.IntField(position, label, intProperty.intValue);
            intProperty.intValue = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }
}
