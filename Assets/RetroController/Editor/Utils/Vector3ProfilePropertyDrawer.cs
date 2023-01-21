using epiplon.Utils;
using UnityEditor;
using UnityEngine;

namespace epipon.Editor
{
    [CustomPropertyDrawer(typeof(Vector3ProfileProperty))]
    public class Vector3ProfilePropertyDrawer : PropertyDrawer
    {
        public void OnEnable()
        {

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var relativeProperty = property.FindPropertyRelative("Value");
            relativeProperty.vector3Value = EditorGUI.Vector3Field(position, label, relativeProperty.vector3Value);
        }
    }
}
