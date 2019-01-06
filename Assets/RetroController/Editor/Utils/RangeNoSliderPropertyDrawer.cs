using UnityEditor;
using UnityEngine;
using vnc.Utils;

namespace vnc.Editor.Utils
{
    [CustomPropertyDrawer(typeof(RangeNoSliderAttribute))]
    public class RangeNoSliderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeNoSliderAttribute rangeAtt = (RangeNoSliderAttribute)attribute;

            if(property.type == "float")
            {
                property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                property.floatValue = Mathf.Clamp(property.floatValue, rangeAtt.minimum, rangeAtt.maximum);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

}
